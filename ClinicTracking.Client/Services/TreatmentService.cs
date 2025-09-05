using ClinicTracking.Client.DTOs;
using System.Net.Http.Json;

namespace ClinicTracking.Client.Services;

public interface ITreatmentService
{
    Task<IEnumerable<TreatmentDto>> GetAllTreatmentsAsync();
    Task<TreatmentDto?> GetTreatmentByIdAsync(Guid id);
    Task<TreatmentDto?> CreateTreatmentAsync(CreateTreatmentDto treatment);
    Task<TreatmentDto?> UpdateTreatmentAsync(Guid id, UpdateTreatmentDto treatment);
    Task<bool> DeleteTreatmentAsync(Guid id);
}

public class TreatmentService : ITreatmentService
{
    private readonly HttpClient _httpClient;

    public TreatmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TreatmentDto>> GetAllTreatmentsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<TreatmentDto>>("api/treatments");
        return response ?? new List<TreatmentDto>();
    }

    public async Task<TreatmentDto?> GetTreatmentByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<TreatmentDto>($"api/treatments/{id}");
    }

    public async Task<TreatmentDto?> CreateTreatmentAsync(CreateTreatmentDto treatment)
    {
        var response = await _httpClient.PostAsJsonAsync("api/treatments", treatment);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TreatmentDto>();
        }
        return null;
    }

    public async Task<TreatmentDto?> UpdateTreatmentAsync(Guid id, UpdateTreatmentDto treatment)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/treatments/{id}", treatment);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TreatmentDto>();
        }
        return null;
    }

    public async Task<bool> DeleteTreatmentAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/treatments/{id}");
        return response.IsSuccessStatusCode;
    }
}