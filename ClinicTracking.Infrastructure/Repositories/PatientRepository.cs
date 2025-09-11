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
            .Include(p => p.TreatmentLookup)
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync();
    }

    public async Task<(IEnumerable<PatientTracking> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .OrderByDescending(p => p.CreatedOn);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<PatientTracking?> GetByIdAsync(Guid id)
    {
        return await _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PatientTracking?> GetByMRNAsync(string mrn)
    {
        return await _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .FirstOrDefaultAsync(p => p.MRN == mrn);
    }

    public async Task<IEnumerable<PatientTracking>> GetAwaitingCounsellingAsync()
    {
        return await _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .Where(p => p.CounsellingDate == null)
            .OrderBy(p => p.ReferralDate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<PatientTracking> Items, int TotalCount)> GetAwaitingCounsellingPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .Where(p => p.CounsellingDate == null)
            .OrderBy(p => p.ReferralDate);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<PatientTracking>> GetAwaitingTreatmentAsync()
    {
        return await _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .Where(p => p.CounsellingDate != null && p.DispensedDate == null)
            .OrderBy(p => p.CounsellingDate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<PatientTracking> Items, int TotalCount)> GetAwaitingTreatmentPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .Where(p => p.CounsellingDate != null && p.DispensedDate == null)
            .OrderBy(p => p.CounsellingDate);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<PatientTracking>> GetFollowUpDueAsync()
    {
        var today = DateTime.Today;
        return await _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .Where(p => (p.NextAppointment != null && p.NextAppointment <= today) ||
                       (p.NextCycleDue != null && p.NextCycleDue <= today))
            .OrderBy(p => p.NextAppointment ?? p.NextCycleDue)
            .ToListAsync();
    }

    public async Task<(IEnumerable<PatientTracking> Items, int TotalCount)> GetFollowUpDuePagedAsync(int pageNumber, int pageSize)
    {
        var today = DateTime.Today;
        var query = _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .Where(p => (p.NextAppointment != null && p.NextAppointment <= today) ||
                       (p.NextCycleDue != null && p.NextCycleDue <= today))
            .OrderBy(p => p.NextAppointment ?? p.NextCycleDue);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<PatientTracking>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return Enumerable.Empty<PatientTracking>();
        }

        var normalizedSearchTerm = searchTerm.ToLower();
        return await _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .Where(p => p.Name.ToLower().Contains(normalizedSearchTerm) || 
                       p.MRN.ToLower().Contains(normalizedSearchTerm))
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync();
    }

    public async Task<(IEnumerable<PatientTracking> Items, int TotalCount)> SearchPagedAsync(string searchTerm, int pageNumber, int pageSize)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return (Enumerable.Empty<PatientTracking>(), 0);
        }

        var normalizedSearchTerm = searchTerm.ToLower();
        var query = _context.PatientTrackings
            .Include(p => p.TreatmentLookup)
            .Where(p => p.Name.ToLower().Contains(normalizedSearchTerm) || 
                       p.MRN.ToLower().Contains(normalizedSearchTerm))
            .OrderByDescending(p => p.CreatedOn);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
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