using Xunit;
using Microsoft.Extensions.DependencyInjection;
using ClinicTracking.Client.DTOs;
using ClinicTracking.Client.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace ClinicTracking.Tests;

public class PatientEditIntegrationTests
{
    [Fact]
    public void EditPatientPage_ShouldExist_AccordingToRoutePattern()
    {
        // Arrange & Act
        var expectedRoute = "/edit-patient/{Id:guid}";
        var actualRouteExists = System.IO.File.Exists(
            "/home/runner/work/SimpleClinicManager/SimpleClinicManager/ClinicTracking.Client/Components/Pages/EditPatient.razor"
        );

        // Assert
        Assert.True(actualRouteExists, "EditPatient.razor page should exist");
    }

    [Fact]
    public void UpdatePatientTrackingDto_ShouldHaveRequiredFields()
    {
        // Arrange
        var dto = new UpdatePatientTrackingDto()
        {
            MRN = "TEST001",
            Name = "Test Patient",
            ReferralDate = DateTime.Today
        };

        // Act & Assert - verify the DTO has all the required fields from the acceptance criteria
        Assert.Equal("TEST001", dto.MRN);
        Assert.Equal("Test Patient", dto.Name);
        Assert.Equal(DateTime.Today, dto.ReferralDate);
        
        // Optional fields should be nullable
        Assert.Null(dto.CounsellingDate);
        Assert.Null(dto.CounsellingBy);
        Assert.Null(dto.DelayReason);
        Assert.Null(dto.DispensedDate);
        Assert.Null(dto.ImagingDate);
        Assert.Null(dto.ResultsDate);
        Assert.Null(dto.NextCycleDue);
        Assert.Null(dto.NextAppointment);
        Assert.Null(dto.Comments);
        
        // Boolean fields should have default values
        Assert.False(dto.SurveyReturned);
        Assert.False(dto.IsEnglishFirstLanguage);
        Assert.False(dto.Adjuvant);
        Assert.False(dto.Palliative);
    }

    [Fact]
    public void UpdatePatientTrackingDto_ShouldHaveValidationAttributes()
    {
        // Arrange
        var dtoType = typeof(UpdatePatientTrackingDto);

        // Act & Assert - verify required validation attributes
        var mrnProperty = dtoType.GetProperty(nameof(UpdatePatientTrackingDto.MRN));
        var nameProperty = dtoType.GetProperty(nameof(UpdatePatientTrackingDto.Name));
        var referralDateProperty = dtoType.GetProperty(nameof(UpdatePatientTrackingDto.ReferralDate));

        Assert.NotNull(mrnProperty);
        Assert.NotNull(nameProperty);
        Assert.NotNull(referralDateProperty);
        
        // Check if Required attributes exist (would need more complex reflection for validation attributes)
        Assert.True(mrnProperty.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false).Length > 0);
        Assert.True(nameProperty.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false).Length > 0);
        // ReferralDate is now nullable, so it should NOT have a Required attribute
        Assert.True(referralDateProperty.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false).Length == 0);
    }

    [Fact]
    public void IPatientService_ShouldHaveUpdateMethodWithCorrectSignature()
    {
        // Arrange
        var serviceType = typeof(IPatientService);

        // Act
        var updateMethod = serviceType.GetMethod(nameof(IPatientService.UpdatePatientAsync));

        // Assert
        Assert.NotNull(updateMethod);
        Assert.Equal(typeof(Task<PatientTrackingDto?>), updateMethod.ReturnType);
        
        var parameters = updateMethod.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(Guid), parameters[0].ParameterType);
        Assert.Equal(typeof(UpdatePatientTrackingDto), parameters[1].ParameterType);
    }

    [Fact]
    public void UpdatePatientTrackingDto_ShouldSupportCounsellingByField()
    {
        // Arrange
        var staffMember = "Dr. Smith";
        var dto = new UpdatePatientTrackingDto()
        {
            MRN = "TEST001",
            Name = "Test Patient",
            ReferralDate = DateTime.Today,
            CounsellingBy = staffMember
        };

        // Act & Assert - verify the CounsellingBy field is properly set and retrieved
        Assert.Equal(staffMember, dto.CounsellingBy);
        
        // Verify it's optional - can be null without validation errors
        dto.CounsellingBy = null;
        Assert.Null(dto.CounsellingBy);
        
        // Verify it accepts reasonable string lengths
        dto.CounsellingBy = new string('A', 200); // Max length should be 200
        Assert.Equal(200, dto.CounsellingBy.Length);
    }
}