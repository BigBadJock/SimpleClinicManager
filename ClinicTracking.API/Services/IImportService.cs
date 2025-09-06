using ClinicTracking.API.DTOs;

namespace ClinicTracking.API.Services;

public interface IImportService
{
    Task<ImportResultDto> ImportFromCsvAsync(Stream fileStream, string fileName, string importedBy);
    Task<bool> ValidateCsvFileAsync(Stream fileStream);
}