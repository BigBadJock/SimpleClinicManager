using ClinicTracking.Core.Interfaces;
using ClinicTracking.Infrastructure.Data;
using ClinicTracking.Infrastructure.Repositories;

namespace ClinicTracking.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ClinicTrackingDbContext _context;
    private PatientRepository? _patientRepository;
    private TreatmentRepository? _treatmentRepository;

    public UnitOfWork(ClinicTrackingDbContext context)
    {
        _context = context;
    }

    public IPatientRepository Patients => _patientRepository ??= new PatientRepository(_context);
    public ITreatmentRepository Treatments => _treatmentRepository ??= new TreatmentRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}