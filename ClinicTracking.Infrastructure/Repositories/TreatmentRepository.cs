using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using ClinicTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicTracking.Infrastructure.Repositories;

public class TreatmentRepository : ITreatmentRepository
{
    private readonly ClinicTrackingDbContext _context;

    public TreatmentRepository(ClinicTrackingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Treatment>> GetAllAsync()
    {
        return await _context.Treatments
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Treatment?> GetByIdAsync(Guid id)
    {
        return await _context.Treatments
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Treatment?> GetByNameAsync(string name)
    {
        return await _context.Treatments
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<bool> IsInUseAsync(Guid id)
    {
        return await _context.PatientTrackings
            .AnyAsync(p => p.TreatmentId == id);
    }

    public async Task AddAsync(Treatment treatment)
    {
        await _context.Treatments.AddAsync(treatment);
    }

    public async Task UpdateAsync(Treatment treatment)
    {
        _context.Treatments.Update(treatment);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var treatment = await GetByIdAsync(id);
        if (treatment != null)
        {
            _context.Treatments.Remove(treatment);
        }
    }
}