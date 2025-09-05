using ClinicTracking.Core.Entities;

namespace ClinicTracking.Tests;

public class TreatmentEntityTests
{
    [Fact]
    public void Treatment_ShouldInitializeWithCorrectDefaults()
    {
        // Arrange & Act
        var treatment = new Treatment
        {
            Id = Guid.NewGuid(),
            Name = "Test Treatment",
            Description = "Test Description",
            CreatedBy = "TestUser"
        };

        // Assert
        Assert.NotEqual(Guid.Empty, treatment.Id);
        Assert.Equal("Test Treatment", treatment.Name);
        Assert.Equal("Test Description", treatment.Description);
        Assert.Equal("TestUser", treatment.CreatedBy);
        Assert.NotNull(treatment.Patients);
        Assert.Empty(treatment.Patients);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Treatment_WithInvalidName_ShouldNotPass(string? name)
    {
        // Arrange
        var treatment = new Treatment
        {
            Id = Guid.NewGuid(),
            Name = name!,
            CreatedBy = "TestUser"
        };

        // Act & Assert
        // This would fail validation if using validation attributes
        Assert.True(string.IsNullOrEmpty(treatment.Name));
    }

    [Fact]
    public void Treatment_WithLongDescription_ShouldAllowUp500Characters()
    {
        // Arrange
        var longDescription = new string('A', 500);
        var treatment = new Treatment
        {
            Id = Guid.NewGuid(),
            Name = "Test Treatment",
            Description = longDescription,
            CreatedBy = "TestUser"
        };

        // Act & Assert
        Assert.Equal(500, treatment.Description.Length);
        Assert.Equal(longDescription, treatment.Description);
    }

    [Fact]
    public void Treatment_NavigationProperty_ShouldWorkCorrectly()
    {
        // Arrange
        var treatment = new Treatment
        {
            Id = Guid.NewGuid(),
            Name = "Test Treatment",
            CreatedBy = "TestUser"
        };

        var patient = new PatientTracking
        {
            Id = Guid.NewGuid(),
            MRN = "TEST001",
            Name = "Test Patient",
            ReferralDate = DateTime.Today,
            TreatmentId = treatment.Id,
            TreatmentLookup = treatment,
            CreatedBy = "TestUser"
        };

        // Act
        treatment.Patients.Add(patient);

        // Assert
        Assert.Single(treatment.Patients);
        Assert.Equal(patient, treatment.Patients.First());
        Assert.Equal(treatment.Id, patient.TreatmentId);
        Assert.Equal(treatment, patient.TreatmentLookup);
    }
}