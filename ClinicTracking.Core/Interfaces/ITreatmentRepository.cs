using ClinicTracking.Core.Entities;

namespace ClinicTracking.Core.Interfaces;

public interface ITreatmentRepository
{
    Task<IEnumerable<Treatment>> GetAllAsync();
    Task<Treatment?> GetByIdAsync(Guid id);
    Task<Treatment?> GetByNameAsync(string name);
    Task<bool> IsInUseAsync(Guid id);
    Task AddAsync(Treatment treatment);
    Task UpdateAsync(Treatment treatment);
    Task DeleteAsync(Guid id);
}