namespace ClinicTracking.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    Task<int> SaveChangesAsync();
}