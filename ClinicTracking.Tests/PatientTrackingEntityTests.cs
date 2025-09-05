using ClinicTracking.Core.Entities;

namespace ClinicTracking.Tests;

public class PatientTrackingEntityTests
{
    [Fact]
    public void PatientTracking_WithNullDates_ShouldCalculateFieldsCorrectly()
    {
        // Arrange
        var patient = new PatientTracking
        {
            Id = Guid.NewGuid(),
            MRN = "TEST001",
            Name = "Test Patient",
            ReferralDate = DateTime.Today,
            // All optional date fields are null
            CounsellingDate = null,
            DispensedDate = null,
            ImagingDate = null,
            ResultsDate = null,
            NextCycleDue = null,
            NextAppointment = null,
            CreatedBy = "TestUser"
        };

        // Act & Assert
        Assert.Null(patient.CounsellingDate);
        Assert.Null(patient.DispensedDate);
        Assert.Null(patient.ImagingDate);
        Assert.Null(patient.ResultsDate);
        Assert.Null(patient.NextCycleDue);
        Assert.Null(patient.NextAppointment);
        
        // Calculated fields should return null when source dates are null
        Assert.Null(patient.WaitTimeReferralToCounselling);
        Assert.Null(patient.TreatTime);
    }

    [Fact]
    public void PatientTracking_WithSomeDates_ShouldCalculateFieldsCorrectly()
    {
        // Arrange
        var patient = new PatientTracking
        {
            Id = Guid.NewGuid(),
            MRN = "TEST002",
            Name = "Test Patient 2",
            ReferralDate = DateTime.Today.AddDays(-10),
            CounsellingDate = DateTime.Today.AddDays(-5),
            DispensedDate = DateTime.Today.AddDays(-2),
            // Other dates remain null
            ImagingDate = null,
            ResultsDate = null,
            NextCycleDue = null,
            NextAppointment = null,
            CreatedBy = "TestUser"
        };

        // Act & Assert
        Assert.NotNull(patient.CounsellingDate);
        Assert.NotNull(patient.DispensedDate);
        Assert.Null(patient.ImagingDate);
        Assert.Null(patient.ResultsDate);
        Assert.Null(patient.NextCycleDue);
        Assert.Null(patient.NextAppointment);
        
        // Calculated fields should work correctly
        Assert.Equal(5, patient.WaitTimeReferralToCounselling);
        Assert.Equal(3, patient.TreatTime);
    }
}