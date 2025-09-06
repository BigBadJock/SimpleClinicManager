using ClinicTracking.API.DTOs;

namespace ClinicTracking.API.Services;

public interface IImportService
{
    Task<ImportResultDto> ImportFromExcelAsync(Stream fileStream, string fileName, string importedBy);
    Task<bool> ValidateExcelFileAsync(Stream fileStream);
}