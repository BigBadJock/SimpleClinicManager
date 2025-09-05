using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using ClinicTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicTracking.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ClinicTrackingDbContext _context;

    public PatientRepository(ClinicTrackingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PatientTracking>> GetAllAsync()
    {
        return await _context.PatientTrackings
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync();
    }

    public async Task<PatientTracking?> GetByIdAsync(Guid id)
    {
        return await _context.PatientTrackings
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PatientTracking?> GetByMRNAsync(string mrn)
    {
        return await _context.PatientTrackings
            .FirstOrDefaultAsync(p => p.MRN == mrn);
    }

    public async Task<IEnumerable<PatientTracking>> GetAwaitingCounsellingAsync()
    {
        return await _context.PatientTrackings
            .Where(p => p.CounsellingDate == null)
            .OrderBy(p => p.ReferralDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientTracking>> GetAwaitingTreatmentAsync()
    {
        return await _context.PatientTrackings
            .Where(p => p.CounsellingDate != null && p.DispensedDate == null)
            .OrderBy(p => p.CounsellingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientTracking>> GetFollowUpDueAsync()
    {
        var today = DateTime.Today;
        return await _context.PatientTrackings
            .Where(p => (p.NextAppointment != null && p.NextAppointment <= today) ||
                       (p.NextCycleDue != null && p.NextCycleDue <= today))
            .OrderBy(p => p.NextAppointment ?? p.NextCycleDue)
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientTracking>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return Enumerable.Empty<PatientTracking>();
        }

        var normalizedSearchTerm = searchTerm.ToLower();
        return await _context.PatientTrackings
            .Where(p => p.Name.ToLower().Contains(normalizedSearchTerm) || 
                       p.MRN.ToLower().Contains(normalizedSearchTerm))
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync();
    }

    public async Task AddAsync(PatientTracking patient)
    {
        await _context.PatientTrackings.AddAsync(patient);
    }

    public async Task UpdateAsync(PatientTracking patient)
    {
        _context.PatientTrackings.Update(patient);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var patient = await GetByIdAsync(id);
        if (patient != null)
        {
            _context.PatientTrackings.Remove(patient);
        }
    }
}