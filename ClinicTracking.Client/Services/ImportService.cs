using ClinicTracking.Client.DTOs;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace ClinicTracking.Client.Services;

public interface IImportService
{
    Task<ImportResultDto?> ImportFromCsvAsync(IBrowserFile file);
    Task<bool> ValidateCsvFileAsync(IBrowserFile file);
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

    public async Task<bool> ValidateCsvFileAsync(IBrowserFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024); // 50MB max
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
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
            _logger.LogError(ex, "Error validating CSV file");
            return false;
        }
    }

    public async Task<ImportResultDto?> ImportFromCsvAsync(IBrowserFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024); // 50MB max
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/import/csv", content);
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
            _logger.LogError(ex, "Error importing CSV file");
            return null;
        }
    }
}