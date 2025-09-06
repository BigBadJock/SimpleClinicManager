using ClinicTracking.Client.DTOs;
using System.Net.Http.Json;

namespace ClinicTracking.Client.Services;

public interface IPatientService
{
    Task<IEnumerable<PatientTrackingDto>> GetAllPatientsAsync();
    Task<PatientTrackingDto?> GetPatientByIdAsync(Guid id);
    Task<PatientTrackingDto?> GetPatientByMRNAsync(string mrn);
    Task<IEnumerable<PatientTrackingDto>> GetAwaitingCounsellingAsync();
    Task<IEnumerable<PatientTrackingDto>> GetAwaitingTreatmentAsync();
    Task<IEnumerable<PatientTrackingDto>> GetFollowUpDueAsync();
    Task<IEnumerable<PatientTrackingDto>> SearchPatientsAsync(string searchTerm);
    Task<PatientTrackingDto?> CreatePatientAsync(CreatePatientTrackingDto patient);
    Task<PatientTrackingDto?> UpdatePatientAsync(Guid id, UpdatePatientTrackingDto patient);
    Task<bool> DeletePatientAsync(Guid id);
    Task<byte[]> ExportToCsvAsync(string? filter = null, string? searchTerm = null);
    Task<byte[]> ExportToExcelAsync(string? filter = null, string? searchTerm = null);
    Task<StatisticsDto?> GetStatisticsAsync(StatisticsFilterDto filter);
}

public class PatientService : IPatientService
{
    private readonly HttpClient _httpClient;

    public PatientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PatientTrackingDto>> GetAllPatientsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<PatientTrackingDto>>("api/patients");
        return response ?? new List<PatientTrackingDto>();
    }

    public async Task<PatientTrackingDto?> GetPatientByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<PatientTrackingDto>($"api/patients/{id}");
    }

    public async Task<PatientTrackingDto?> GetPatientByMRNAsync(string mrn)
    {
        return await _httpClient.GetFromJsonAsync<PatientTrackingDto>($"api/patients/mrn/{mrn}");
    }

    public async Task<IEnumerable<PatientTrackingDto>> GetAwaitingCounsellingAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<PatientTrackingDto>>("api/patients/awaiting-counselling");
        return response ?? new List<PatientTrackingDto>();
    }

    public async Task<IEnumerable<PatientTrackingDto>> GetAwaitingTreatmentAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<PatientTrackingDto>>("api/patients/awaiting-treatment");
        return response ?? new List<PatientTrackingDto>();
    }

    public async Task<IEnumerable<PatientTrackingDto>> GetFollowUpDueAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<PatientTrackingDto>>("api/patients/follow-up-due");
        return response ?? new List<PatientTrackingDto>();
    }

    public async Task<IEnumerable<PatientTrackingDto>> SearchPatientsAsync(string searchTerm)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<PatientTrackingDto>>($"api/patients/search/{Uri.EscapeDataString(searchTerm)}");
        return response ?? new List<PatientTrackingDto>();
    }

    public async Task<PatientTrackingDto?> CreatePatientAsync(CreatePatientTrackingDto patient)
    {
        var response = await _httpClient.PostAsJsonAsync("api/patients", patient);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PatientTrackingDto>();
        }
        return null;
    }

    public async Task<PatientTrackingDto?> UpdatePatientAsync(Guid id, UpdatePatientTrackingDto patient)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/patients/{id}", patient);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PatientTrackingDto>();
        }
        return null;
    }

    public async Task<bool> DeletePatientAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/patients/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<byte[]> ExportToCsvAsync(string? filter = null, string? searchTerm = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(filter))
            queryParams.Add($"filter={Uri.EscapeDataString(filter)}");
        if (!string.IsNullOrEmpty(searchTerm))
            queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
        
        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        var response = await _httpClient.GetAsync($"api/patients/export/csv{queryString}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> ExportToExcelAsync(string? filter = null, string? searchTerm = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(filter))
            queryParams.Add($"filter={Uri.EscapeDataString(filter)}");
        if (!string.IsNullOrEmpty(searchTerm))
            queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
        
        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        var response = await _httpClient.GetAsync($"api/patients/export/excel{queryString}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();

    public async Task<StatisticsDto?> GetStatisticsAsync(StatisticsFilterDto filter)
    {
        var response = await _httpClient.PostAsJsonAsync("api/patients/statistics", filter);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<StatisticsDto>();
        }
        return null;

    }
}