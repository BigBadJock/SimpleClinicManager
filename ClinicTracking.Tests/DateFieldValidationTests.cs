using System.ComponentModel.DataAnnotations;
using ClinicTracking.Client.DTOs;
using ClinicTracking.API.DTOs;

namespace ClinicTracking.Tests;

public class DateFieldValidationTests
{
    [Fact]
    public void CreatePatientTrackingDto_Client_WithNullDates_ShouldBeValid()
    {
        // Arrange
        var dto = new ClinicTracking.Client.DTOs.CreatePatientTrackingDto
        {
            MRN = "TEST001",
            Name = "Test Patient",
            ReferralDate = DateTime.Today,
            // All the optional date fields left as null
            CounsellingDate = null,
            DispensedDate = null,
            ImagingDate = null,
            ResultsDate = null,
            NextCycleDue = null,
            NextAppointment = null
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, validationResults, true);

        // Assert
        Assert.True(isValid, $"Validation failed: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
        Assert.Empty(validationResults);
    }

    [Fact]
    public void CreatePatientTrackingDto_API_WithNullDates_ShouldBeValid()
    {
        // Arrange
        var dto = new ClinicTracking.API.DTOs.CreatePatientTrackingDto
        {
            MRN = "TEST001",
            Name = "Test Patient",
            ReferralDate = DateTime.Today,
            // All the optional date fields left as null
            CounsellingDate = null,
            DispensedDate = null,
            ImagingDate = null,
            ResultsDate = null,
            NextCycleDue = null,
            NextAppointment = null
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, validationResults, true);

        // Assert
        Assert.True(isValid, $"Validation failed: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
        Assert.Empty(validationResults);
    }

    [Fact]
    public void UpdatePatientTrackingDto_WithNullDates_ShouldBeValid()
    {
        // Arrange
        var dto = new ClinicTracking.API.DTOs.UpdatePatientTrackingDto
        {
            MRN = "TEST001",
            Name = "Test Patient",
            ReferralDate = DateTime.Today,
            // All the optional date fields left as null
            CounsellingDate = null,
            DispensedDate = null,
            ImagingDate = null,
            ResultsDate = null,
            NextCycleDue = null,
            NextAppointment = null
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(dto);
        var isValid = Validator.TryValidateObject(dto, context, validationResults, true);

        // Assert
        Assert.True(isValid, $"Validation failed: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
        Assert.Empty(validationResults);
    }
}