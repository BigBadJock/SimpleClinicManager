namespace ClinicTracking.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    ITreatmentRepository Treatments { get; }
    Task<int> SaveChangesAsync();
}