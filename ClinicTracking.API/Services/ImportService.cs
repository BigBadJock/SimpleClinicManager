using ClinicTracking.API.DTOs;
using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using OfficeOpenXml;
using System.Globalization;

namespace ClinicTracking.API.Services;

public class ImportService : IImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ImportService> _logger;

    // Expected Excel headers
    private readonly string[] _expectedHeaders = new[]
    {
        "Referral date", "Counselling date", "Delay Reason", "Survey", "Wait time from ref",
        "english 1st language", "Name", "MRn", "Treatment", "Adjuvant", "Palliative",
        "Dispensed", "Treat Time", "Imaging", "Results", "Next cycle due", "Next appt", "comments"
    };

    static ImportService()
    {
        // Set EPPlus license once for the entire application
        try
        {
            #pragma warning disable CS0618
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            #pragma warning restore CS0618
        }
        catch
        {
            // License already set or not supported in this version
        }
    }

    public ImportService(IUnitOfWork unitOfWork, ILogger<ImportService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task<bool> ValidateExcelFileAsync(Stream fileStream)
    {
        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null || worksheet.Cells.Any() == false)
                return Task.FromResult(false);

            // Check if it has at least some expected headers
            var headerRow = 1;
            var actualHeaders = new List<string>();
            
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var headerValue = worksheet.Cells[headerRow, col].Text?.Trim();
                if (!string.IsNullOrEmpty(headerValue))
                    actualHeaders.Add(headerValue);
            }

            // Check if we have at least the mandatory headers (Name and MRN)
            var hasName = actualHeaders.Any(h => h.Equals("Name", StringComparison.OrdinalIgnoreCase));
            var hasMrn = actualHeaders.Any(h => h.Equals("MRn", StringComparison.OrdinalIgnoreCase) || 
                                              h.Equals("MRN", StringComparison.OrdinalIgnoreCase));
            
            return Task.FromResult(hasName && hasMrn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Excel file");
            return Task.FromResult(false);
        }
    }

    public async Task<ImportResultDto> ImportFromExcelAsync(Stream fileStream, string fileName, string importedBy)
    {
        var result = new ImportResultDto
        {
            ImportedAt = DateTime.UtcNow,
            ImportedBy = importedBy
        };

        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
            {
                result.Errors.Add("No worksheet found in the Excel file");
                return result;
            }

            var headerMappings = MapHeaders(worksheet);
            if (headerMappings.Count == 0)
            {
                result.Errors.Add("Could not find required headers (Name and MRN) in the Excel file");
                return result;
            }

            result.TotalRows = worksheet.Dimension.End.Row - 1; // Exclude header row

            // Process data rows
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                await ProcessDataRow(worksheet, row, headerMappings, result);
            }

            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Import completed. Success: {Success}, Skipped: {Skipped}, Errors: {Errors}",
                result.SuccessfulImports, result.SkippedRows, result.Errors.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Excel import");
            result.Errors.Add($"Import failed: {ex.Message}");
            return result;
        }
    }

    private Dictionary<string, int> MapHeaders(ExcelWorksheet worksheet)
    {
        var mappings = new Dictionary<string, int>();
        
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            var header = worksheet.Cells[1, col].Text?.Trim();
            if (!string.IsNullOrEmpty(header))
            {
                // Map common variations
                var normalizedHeader = NormalizeHeader(header);
                mappings[normalizedHeader] = col;
            }
        }

        return mappings;
    }

    private string NormalizeHeader(string header)
    {
        return header.ToLowerInvariant().Replace(" ", "").Replace("_", "");
    }

    private async Task ProcessDataRow(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMappings, ImportResultDto result)
    {
        try
        {
            // Extract required fields
            var mrn = GetCellValue(worksheet, row, headerMappings, "mrn");
            var name = GetCellValue(worksheet, row, headerMappings, "name");

            if (string.IsNullOrWhiteSpace(mrn) || string.IsNullOrWhiteSpace(name))
            {
                result.SkippedRows++;
                result.Warnings.Add($"Row {row}: Skipped - Missing required MRN or Name");
                return;
            }

            // Check if patient already exists
            var existingPatient = await _unitOfWork.Patients.GetByMRNAsync(mrn);
            if (existingPatient != null)
            {
                result.SkippedRows++;
                result.Warnings.Add($"Row {row}: Skipped - Patient with MRN '{mrn}' already exists");
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
            ProcessDateField(worksheet, row, headerMappings, "referraldate", v => patient.ReferralDate = v ?? DateTime.Today, comments);
            ProcessDateField(worksheet, row, headerMappings, "counsellingdate", v => patient.CounsellingDate = v, comments);
            
            patient.DelayReason = GetCellValue(worksheet, row, headerMappings, "delayreason");
            
            ProcessBooleanField(worksheet, row, headerMappings, "survey", v => patient.SurveyReturned = v ?? false, comments);
            ProcessBooleanField(worksheet, row, headerMappings, "english1stlanguage", v => patient.IsEnglishFirstLanguage = v ?? true, comments);
            ProcessBooleanField(worksheet, row, headerMappings, "adjuvant", v => patient.Adjuvant = v ?? false, comments);
            ProcessBooleanField(worksheet, row, headerMappings, "palliative", v => patient.Palliative = v ?? false, comments);

            // Process treatment with auto-creation
            await ProcessTreatmentField(worksheet, row, headerMappings, patient, result, comments);

            ProcessDateField(worksheet, row, headerMappings, "dispensed", v => patient.DispensedDate = v, comments);
            ProcessDateField(worksheet, row, headerMappings, "imaging", v => patient.ImagingDate = v, comments);
            ProcessDateField(worksheet, row, headerMappings, "results", v => patient.ResultsDate = v, comments);
            ProcessDateField(worksheet, row, headerMappings, "nextcycledue", v => patient.NextCycleDue = v, comments);
            ProcessDateField(worksheet, row, headerMappings, "nextappt", v => patient.NextAppointment = v, comments);

            // Combine existing comments with validation comments
            var existingComments = GetCellValue(worksheet, row, headerMappings, "comments");
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
            result.Errors.Add($"Row {row}: Error processing - {ex.Message}");
            _logger.LogError(ex, "Error processing row {Row}", row);
        }
    }

    private string? GetCellValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMappings, string headerKey)
    {
        if (headerMappings.TryGetValue(headerKey, out int col))
        {
            return worksheet.Cells[row, col].Text?.Trim();
        }
        return null;
    }

    private void ProcessDateField(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMappings, 
        string headerKey, Action<DateTime?> setter, List<string> comments)
    {
        var value = GetCellValue(worksheet, row, headerMappings, headerKey);
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

    private void ProcessBooleanField(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMappings,
        string headerKey, Action<bool?> setter, List<string> comments)
    {
        var value = GetCellValue(worksheet, row, headerMappings, headerKey);
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

    private async Task ProcessTreatmentField(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMappings,
        PatientTracking patient, ImportResultDto result, List<string> comments)
    {
        var treatmentName = GetCellValue(worksheet, row, headerMappings, "treatment");
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