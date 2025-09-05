using ClinicTracking.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicTracking.Infrastructure.Data;

public class ClinicTrackingDbContext : DbContext
{
    public ClinicTrackingDbContext(DbContextOptions<ClinicTrackingDbContext> options) : base(options)
    {
    }


    public DbSet<PatientTracking> PatientTrackings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PatientTracking>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MRN)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(e => e.MRN)
                .IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.ReferralDate)
                .IsRequired();

            entity.Property(e => e.CounsellingBy)
                .HasMaxLength(200);

            entity.Property(e => e.DelayReason)
                .HasMaxLength(500);

            entity.Property(e => e.SurveyReturned)
                .HasDefaultValue(false);

            entity.Property(e => e.IsEnglishFirstLanguage)
                .HasDefaultValue(true);

            entity.Property(e => e.Treatment)
                .HasMaxLength(100);

            entity.Property(e => e.Adjuvant)
                .HasDefaultValue(false);

            entity.Property(e => e.Palliative)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(256);

            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            // Ignore calculated properties as they are not stored in the database
            entity.Ignore(e => e.WaitTimeReferralToCounselling);
            entity.Ignore(e => e.TreatTime);
        });
    }
}