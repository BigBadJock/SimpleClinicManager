using ClinicTracking.Core.Entities;

namespace ClinicTracking.Core.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<PatientTracking>> GetAllAsync();
    Task<PatientTracking?> GetByIdAsync(Guid id);
    Task<PatientTracking?> GetByMRNAsync(string mrn);
    Task<IEnumerable<PatientTracking>> GetAwaitingCounsellingAsync();
    Task<IEnumerable<PatientTracking>> GetAwaitingTreatmentAsync();
    Task<IEnumerable<PatientTracking>> GetFollowUpDueAsync();
    Task<IEnumerable<PatientTracking>> SearchAsync(string searchTerm);
    Task AddAsync(PatientTracking patient);
    Task UpdateAsync(PatientTracking patient);
    Task DeleteAsync(Guid id);
}