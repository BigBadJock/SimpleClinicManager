using ClinicTracking.API.DTOs;
using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using System.Globalization;

namespace ClinicTracking.API.Services;

public class ImportService : IImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ImportService> _logger;

    // Expected CSV headers
    private readonly string[] _expectedHeaders = new[]
    {
        "Referral date", "Counselling date", "Delay Reason", "Survey", "Wait time from ref",
        "english 1st language", "Name", "MRn", "Treatment", "Adjuvant", "Palliative",
        "Dispensed", "Treat Time", "Imaging", "Results", "Next cycle due", "Next appt", "comments"
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
            
            // Read first line to get headers
            var headerLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(headerLine))
                return Task.FromResult(false);

            var headers = ParseCsvLine(headerLine);
            
            // Check if we have at least the mandatory headers (Name and MRN)
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
            
            // Read header line
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

            // Count total rows first
            var allLines = new List<string>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    allLines.Add(line);
            }

            result.TotalRows = allLines.Count;

            // Process data rows
            for (int i = 0; i < allLines.Count; i++)
            {
                var rowNumber = i + 2; // +2 because row 1 is header and we're 0-indexed
                await ProcessDataRow(allLines[i], headers, headerMappings, rowNumber, result);
            }

            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Import completed. Success: {Success}, Skipped: {Skipped}, Errors: {Errors}",
                result.SuccessfulImports, result.SkippedRows, result.Errors.Count);

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
                    // Escaped quote
                    currentField.Append('"');
                    i++; // Skip next quote
                }
                else
                {
                    // Toggle quote state
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // Field separator
                result.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        // Add the last field
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
                // Map common variations
                var normalizedHeader = NormalizeHeader(header);
                mappings[normalizedHeader] = i;
            }
        }

        return mappings;
    }

    private string NormalizeHeader(string header)
    {
        return header.ToLowerInvariant().Replace(" ", "").Replace("_", "");
    }

    private async Task ProcessDataRow(string csvLine, string[] headers, Dictionary<string, int> headerMappings, int rowNumber, ImportResultDto result)
    {
        try
        {
            var fields = ParseCsvLine(csvLine);
            
            // Extract required fields
            var mrn = GetCellValue(fields, headerMappings, "mrn");
            var name = GetCellValue(fields, headerMappings, "name");

            if (string.IsNullOrWhiteSpace(mrn) || string.IsNullOrWhiteSpace(name))
            {
                result.SkippedRows++;
                result.Warnings.Add($"Row {rowNumber}: Skipped - Missing required MRN or Name");
                return;
            }

            // Check if patient already exists
            var existingPatient = await _unitOfWork.Patients.GetByMRNAsync(mrn);
            if (existingPatient != null)
            {
                result.SkippedRows++;
                result.Warnings.Add($"Row {rowNumber}: Skipped - Patient with MRN '{mrn}' already exists");
                return;
            }

            var patient = new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = mrn,
                Name = name,
                CreatedBy = result.ImportedBy,
                CreatedOn = DateTime.UtcNow
            };

            var comments = new List<string>();

            // Process all fields with validation
            ProcessDateField(fields, headerMappings, "referraldate", v => patient.ReferralDate = v ?? DateTime.Today, comments);
            ProcessDateField(fields, headerMappings, "counsellingdate", v => patient.CounsellingDate = v, comments);
            
            patient.DelayReason = GetCellValue(fields, headerMappings, "delayreason");
            
            ProcessBooleanField(fields, headerMappings, "survey", v => patient.SurveyReturned = v ?? false, comments);
            ProcessBooleanField(fields, headerMappings, "english1stlanguage", v => patient.IsEnglishFirstLanguage = v ?? true, comments);
            ProcessBooleanField(fields, headerMappings, "adjuvant", v => patient.Adjuvant = v ?? false, comments);
            ProcessBooleanField(fields, headerMappings, "palliative", v => patient.Palliative = v ?? false, comments);

            // Process treatment with auto-creation
            await ProcessTreatmentField(fields, headerMappings, patient, result, comments);

            ProcessDateField(fields, headerMappings, "dispensed", v => patient.DispensedDate = v, comments);
            ProcessDateField(fields, headerMappings, "imaging", v => patient.ImagingDate = v, comments);
            ProcessDateField(fields, headerMappings, "results", v => patient.ResultsDate = v, comments);
            ProcessDateField(fields, headerMappings, "nextcycledue", v => patient.NextCycleDue = v, comments);
            ProcessDateField(fields, headerMappings, "nextappt", v => patient.NextAppointment = v, comments);

            // Combine existing comments with validation comments
            var existingComments = GetCellValue(fields, headerMappings, "comments");
            var allComments = new List<string>();
            if (!string.IsNullOrWhiteSpace(existingComments))
                allComments.Add(existingComments);
            allComments.AddRange(comments);
            
            patient.Comments = string.Join("; ", allComments);

            await _unitOfWork.Patients.AddAsync(patient);
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

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            setter(date);
        }
        else
        {
            setter(null);
            comments.Add($"Invalid date value '{value}' for {headerKey}");
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
        if (normalizedValue == "true" || normalizedValue == "yes" || normalizedValue == "1")
        {
            setter(true);
        }
        else if (normalizedValue == "false" || normalizedValue == "no" || normalizedValue == "0")
        {
            setter(false);
        }
        else
        {
            setter(null);
            comments.Add($"Invalid boolean value '{value}' for {headerKey}");
        }
    }

    private async Task ProcessTreatmentField(string[] fields, Dictionary<string, int> headerMappings,
        PatientTracking patient, ImportResultDto result, List<string> comments)
    {
        var treatmentName = GetCellValue(fields, headerMappings, "treatment");
        if (string.IsNullOrWhiteSpace(treatmentName))
            return;

        // Look up existing treatment
        var existingTreatment = await _unitOfWork.Treatments.GetByNameAsync(treatmentName);
        if (existingTreatment != null)
        {
            patient.TreatmentId = existingTreatment.Id;
            return;
        }

        // Create new treatment
        var newTreatment = new Treatment
        {
            Id = Guid.NewGuid(),
            Name = treatmentName,
            IsAutoAdded = true,
            CreatedBy = "import",
            CreatedOn = DateTime.UtcNow
        };

        await _unitOfWork.Treatments.AddAsync(newTreatment);
        patient.TreatmentId = newTreatment.Id;
        
        result.NewTreatmentsAdded++;
        result.NewTreatmentNames.Add(treatmentName);
        comments.Add($"New treatment auto-added during import: {treatmentName}");
    }
}