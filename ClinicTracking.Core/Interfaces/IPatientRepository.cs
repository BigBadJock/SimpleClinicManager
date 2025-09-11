using ClinicTracking.Core.Entities;

namespace ClinicTracking.Core.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<PatientTracking>> GetAllAsync();
    Task<(IEnumerable<PatientTracking> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<PatientTracking?> GetByIdAsync(Guid id);
    Task<PatientTracking?> GetByMRNAsync(string mrn);
    Task<IEnumerable<PatientTracking>> GetAwaitingCounsellingAsync();
    Task<(IEnumerable<PatientTracking> Items, int TotalCount)> GetAwaitingCounsellingPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<PatientTracking>> GetAwaitingTreatmentAsync();
    Task<(IEnumerable<PatientTracking> Items, int TotalCount)> GetAwaitingTreatmentPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<PatientTracking>> GetFollowUpDueAsync();
    Task<(IEnumerable<PatientTracking> Items, int TotalCount)> GetFollowUpDuePagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<PatientTracking>> SearchAsync(string searchTerm);
    Task<(IEnumerable<PatientTracking> Items, int TotalCount)> SearchPagedAsync(string searchTerm, int pageNumber, int pageSize);
    Task AddAsync(PatientTracking patient);
    Task UpdateAsync(PatientTracking patient);
    Task DeleteAsync(Guid id);
}