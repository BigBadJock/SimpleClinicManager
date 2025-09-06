using ClinicTracking.Client.DTOs;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace ClinicTracking.Client.Services;

public interface IImportService
{
    Task<ImportResultDto?> ImportFromExcelAsync(IBrowserFile file);
    Task<bool> ValidateExcelFileAsync(IBrowserFile file);
}

public class ImportService : IImportService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ImportService> _logger;

    public ImportService(HttpClient httpClient, ILogger<ImportService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ValidateExcelFileAsync(IBrowserFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024); // 50MB max
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/import/validate", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Excel file");
            return false;
        }
    }

    public async Task<ImportResultDto?> ImportFromExcelAsync(IBrowserFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024); // 50MB max
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/import/excel", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ImportResultDto>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Import failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
            
            // Try to parse as ImportResultDto in case of controlled errors
            try
            {
                return await response.Content.ReadFromJsonAsync<ImportResultDto>();
            }
            catch
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Excel file");
            return null;
        }
    }
}