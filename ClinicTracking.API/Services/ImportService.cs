using ClinicTracking.API.DTOs;
using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using System.Globalization;

namespace ClinicTracking.API.Services;

/// <summary>
/// Handles validation and import of patient tracking data from a CSV file.
/// Responsibilities:
///  - Basic header validation (checks for MRN & Name)
///  - Row parsing with tolerant CSV handling (quotes, embedded commas)
///  - Data mapping & normalization (dates, booleans, treatments)
///  - Automatic creation of missing Treatment records (flagged as IsAutoAdded)
///  - Collects rich result details (successes, skips, new treatments, warnings, errors)
/// </summary>
public class ImportService : IImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ImportService> _logger;

    private static readonly string[] DateFormats =
    {
        "dd/MM/yyyy","d/M/yyyy","dd/M/yyyy","d/MM/yyyy",
        "dd/MM/yy","d/M/yy",
        "yyyy-MM-dd",
        "dd-MM-yyyy","d-M-yyyy",
        "dd.MM.yyyy","d.M.yyyy"
    };

    private readonly string[] _expectedHeaders =
    {
        "Referral date","Counselling date","Delay Reason","Survey","Wait time from ref",
        "english 1st language","Name","MRn","Treatment","Adjuvant","Palliative",
        "Dispensed","Treat Time","Imaging","Results","Next cycle due","Next appt","comments"
    };

    public ImportService(IUnitOfWork unitOfWork, ILogger<ImportService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<bool> ValidateCsvFileAsync(Stream fileStream)
    {
        try
        {
            fileStream.Position = 0;
            using var reader = new StreamReader(fileStream);

            var headerLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(headerLine))
                return Task.FromResult(false);

            var headers = ParseCsvLine(headerLine);
            var hasName = headers.Any(h => h.Equals("Name", StringComparison.OrdinalIgnoreCase));
            var hasMrn = headers.Any(h => h.Equals("MRn", StringComparison.OrdinalIgnoreCase) ||
                                          h.Equals("MRN", StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(hasName && hasMrn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CSV file");
            return Task.FromResult(false);
        }
    }

    public async Task<ImportResultDto> ImportFromCsvAsync(Stream fileStream, string fileName, string importedBy)
    {
        var result = new ImportResultDto
        {
            ImportedAt = DateTime.UtcNow,
            ImportedBy = importedBy
        };

        try
        {
            fileStream.Position = 0;
            using var reader = new StreamReader(fileStream);

            var headerLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                result.Errors.Add("CSV file is empty or has no header row");
                return result;
            }

            var headers = ParseCsvLine(headerLine);
            var headerMappings = MapHeaders(headers);
            if (headerMappings.Count == 0)
            {
                result.Errors.Add("Could not find required headers (Name and MRN) in the CSV file");
                return result;
            }

            // Pre-load treatments into cache (normalized name -> Id)
            var existingTreatments = await _unitOfWork.Treatments.GetAllAsync();
            var treatmentCache = existingTreatments
                .GroupBy(t => t.Name.ToUpperInvariant())
                .ToDictionary(g => g.Key, g => g.First().Id);

            // Track MRNs successfully queued in THIS import to avoid duplicates in the same file
            var processedMrns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var allLines = new List<string>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    allLines.Add(line);
            }

            result.TotalRows = allLines.Count;

            for (int i = 0; i < allLines.Count; i++)
            {
                var rowNumber = i + 2;
                await ProcessDataRow(allLines[i], headers, headerMappings, rowNumber, result, treatmentCache, processedMrns);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Import completed. Success: {Success}, Skipped: {Skipped}, Errors: {Errors}, NewTreatments: {NewTreatments}",
                result.SuccessfulImports, result.SkippedRows, result.Errors.Count, result.NewTreatmentsAdded);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during CSV import");
            result.Errors.Add($"Import failed: {ex.Message}");
            return result;
        }
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var inQuotes = false;
        var currentField = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        result.Add(currentField.ToString());
        return result.ToArray();
    }

    private Dictionary<string, int> MapHeaders(string[] headers)
    {
        var mappings = new Dictionary<string, int>();

        for (int i = 0; i < headers.Length; i++)
        {
            var header = headers[i]?.Trim();
            if (!string.IsNullOrEmpty(header))
            {
                var normalizedHeader = NormalizeHeader(header);
                mappings[normalizedHeader] = i;
            }
        }

        return mappings;
    }

    private string NormalizeHeader(string header) =>
        header.ToLowerInvariant().Replace(" ", "").Replace("_", "");

    private static string NormalizeMrn(string mrn) => mrn.Trim().ToUpperInvariant();

    private async Task ProcessDataRow(
        string csvLine,
        string[] headers,
        Dictionary<string, int> headerMappings,
        int rowNumber,
        ImportResultDto result,
        Dictionary<string, Guid> treatmentCache,
        HashSet<string> processedMrns)
    {
        try
        {
            var fields = ParseCsvLine(csvLine);

            var mrnRaw = GetCellValue(fields, headerMappings, "mrn");
            var name = GetCellValue(fields, headerMappings, "name");

            if (string.IsNullOrWhiteSpace(mrnRaw) || string.IsNullOrWhiteSpace(name))
            {
                result.SkippedRows++;
                result.Warnings.Add($"Row {rowNumber}: Skipped - Missing required MRN or Name");
                return;
            }

            var normalizedMrn = NormalizeMrn(mrnRaw);

            // Duplicate within the same file (second occurrence in this batch)
            if (processedMrns.Contains(normalizedMrn))
            {
                result.SkippedRows++;
                result.Warnings.Add($"Row {rowNumber}: Skipped - Duplicate MRN '{mrnRaw}' already imported earlier in this file");
                return;
            }

            // Existing in database
            var existingPatient = await _unitOfWork.Patients.GetByMRNAsync(mrnRaw);
            if (existingPatient != null)
            {
                result.SkippedRows++;
                result.Warnings.Add($"Row {rowNumber}: Skipped - Patient with MRN '{mrnRaw}' already exists in database");
                return;
            }

            var patient = new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = mrnRaw.Trim(),
                Name = name.Trim(),
                CreatedBy = result.ImportedBy,
                CreatedOn = DateTime.UtcNow
            };

            var comments = new List<string>();

            ProcessDateField(fields, headerMappings, "referraldate", v => patient.ReferralDate = v, comments);
            ProcessDateField(fields, headerMappings, "counsellingdate", v => patient.CounsellingDate = v, comments);

            patient.DelayReason = GetCellValue(fields, headerMappings, "delayreason");

            ProcessBooleanField(fields, headerMappings, "survey", v => patient.SurveyReturned = v ?? false, comments);
            ProcessBooleanField(fields, headerMappings, "english1stlanguage", v => patient.IsEnglishFirstLanguage = v ?? true, comments);
            ProcessBooleanField(fields, headerMappings, "adjuvant", v => patient.Adjuvant = v ?? false, comments);
            ProcessBooleanField(fields, headerMappings, "palliative", v => patient.Palliative = v ?? false, comments);

            await ProcessTreatmentField(fields, headerMappings, patient, result, comments, treatmentCache);

            ProcessDateField(fields, headerMappings, "dispensed", v => patient.DispensedDate = v, comments);
            ProcessDateField(fields, headerMappings, "imaging", v => patient.ImagingDate = v, comments);
            ProcessDateField(fields, headerMappings, "results", v => patient.ResultsDate = v, comments);
            ProcessDateField(fields, headerMappings, "nextcycledue", v => patient.NextCycleDue = v, comments);
            ProcessDateField(fields, headerMappings, "nextappt", v => patient.NextAppointment = v, comments);

            var existingComments = GetCellValue(fields, headerMappings, "comments");
            var allComments = new List<string>();
            if (!string.IsNullOrWhiteSpace(existingComments))
                allComments.Add(existingComments);
            allComments.AddRange(comments);

            patient.Comments = string.Join("; ", allComments);

            await _unitOfWork.Patients.AddAsync(patient);
            processedMrns.Add(normalizedMrn); // mark only after successful queue
            result.SuccessfulImports++;
        }
        catch (Exception ex)
        {
            result.SkippedRows++;
            result.Errors.Add($"Row {rowNumber}: Error processing - {ex.Message}");
            _logger.LogError(ex, "Error processing row {Row}", rowNumber);
        }
    }

    private string? GetCellValue(string[] fields, Dictionary<string, int> headerMappings, string headerKey)
    {
        if (headerMappings.TryGetValue(headerKey, out int col) && col < fields.Length)
        {
            return fields[col]?.Trim();
        }
        return null;
    }

    private void ProcessDateField(string[] fields, Dictionary<string, int> headerMappings,
        string headerKey, Action<DateTime?> setter, List<string> comments)
    {
        var value = GetCellValue(fields, headerMappings, headerKey);
        if (string.IsNullOrWhiteSpace(value))
        {
            setter(null);
            return;
        }

        var trimmed = value.Trim();

        if (double.TryParse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture, out var oa))
        {
            try
            {
                var dateFromOa = DateTime.FromOADate(oa);
                setter(dateFromOa);
                return;
            }
            catch { }
        }

        var normalized = trimmed.Replace('-', '/').Replace('.', '/');

        if (DateTime.TryParseExact(normalized, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            setter(parsed);
        }
        else
        {
            setter(null);
            comments.Add($"Invalid date value '{value}' for {headerKey} (expected dd/MM/yyyy)");
        }
    }

    private void ProcessBooleanField(string[] fields, Dictionary<string, int> headerMappings,
        string headerKey, Action<bool?> setter, List<string> comments)
    {
        var value = GetCellValue(fields, headerMappings, headerKey);
        if (string.IsNullOrWhiteSpace(value))
        {
            setter(null);
            return;
        }

        var normalizedValue = value.ToLowerInvariant();
        if (normalizedValue is "true" or "yes" or "1" or "y")
        {
            setter(true);
        }
        else if (normalizedValue is "false" or "no" or "0" or "n")
        {
            setter(false);
        }
        else
        {
            setter(null);
            comments.Add($"Invalid boolean value '{value}' for {headerKey}");
        }
    }

    private async Task ProcessTreatmentField(
        string[] fields,
        Dictionary<string, int> headerMappings,
        PatientTracking patient,
        ImportResultDto result,
        List<string> comments,
        Dictionary<string, Guid> treatmentCache)
    {
        var treatmentName = GetCellValue(fields, headerMappings, "treatment");
        if (string.IsNullOrWhiteSpace(treatmentName))
            return;

        var key = treatmentName.Trim().ToUpperInvariant();

        if (treatmentCache.TryGetValue(key, out var cachedId))
        {
            patient.TreatmentId = cachedId;
            return;
        }

        var existingTreatment = await _unitOfWork.Treatments.GetByNameAsync(treatmentName);
        if (existingTreatment != null)
        {
            treatmentCache[key] = existingTreatment.Id;
            patient.TreatmentId = existingTreatment.Id;
            return;
        }

        var newTreatment = new Treatment
        {
            Id = Guid.NewGuid(),
            Name = treatmentName.Trim(),
            IsAutoAdded = true,
            CreatedBy = "import",
            CreatedOn = DateTime.UtcNow
        };

        await _unitOfWork.Treatments.AddAsync(newTreatment);
        treatmentCache[key] = newTreatment.Id;
        patient.TreatmentId = newTreatment.Id;

        result.NewTreatmentsAdded++;
        result.NewTreatmentNames.Add(treatmentName.Trim());
        comments.Add($"New treatment auto-added during import: {treatmentName.Trim()}");
    }
}