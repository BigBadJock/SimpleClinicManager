using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicTracking.Infrastructure.Data
{
    public class ClinicTrackingDbContextFactory : IDesignTimeDbContextFactory<ClinicTrackingDbContext>
    {
        public ClinicTrackingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClinicTrackingDbContext>();

            // Use your Azure SQL connection string here for design-time migrations
            optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=ClinicTracking;User ID=sa;Password=D3v3l0p3r@SQL;TrustServerCertificate=True;MultipleActiveResultSets=true;");


            return new ClinicTrackingDbContext(optionsBuilder.Options);
        }
    }
}
