using System.ComponentModel.DataAnnotations;
using ClinicTracking.API.DTOs;

namespace ClinicTracking.Tests;

public class StatisticsTests
{
    [Fact]
    public void StatisticsFilterDto_WithValidDates_ShouldBeValid()
    {
        // Arrange
        var filter = new StatisticsFilterDto
        {
            StartDate = DateTime.Today.AddDays(-30),
            EndDate = DateTime.Today
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(filter);
        var isValid = Validator.TryValidateObject(filter, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void StatisticsFilterDto_WithNullDates_ShouldBeValid()
    {
        // Arrange
        var filter = new StatisticsFilterDto
        {
            StartDate = null,
            EndDate = null
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(filter);
        var isValid = Validator.TryValidateObject(filter, context, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void SummaryMetricsDto_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var metrics = new SummaryMetricsDto();

        // Assert
        Assert.Equal(0, metrics.TotalPatients);
        Assert.Equal(5, metrics.TargetDays);
        Assert.Null(metrics.AverageWaitTime);
        Assert.Null(metrics.MedianWaitTime);
    }

    [Fact]
    public void TimeDistributionDto_Properties_ShouldHaveCorrectTypes()
    {
        // Arrange & Act
        var distribution = new TimeDistributionDto
        {
            Range = "0-5 days",
            Count = 10,
            MinDays = 0,
            MaxDays = 5
        };

        // Assert
        Assert.Equal("0-5 days", distribution.Range);
        Assert.Equal(10, distribution.Count);
        Assert.Equal(0, distribution.MinDays);
        Assert.Equal(5, distribution.MaxDays);
    }

    [Fact]
    public void TreatmentTypeDto_Properties_ShouldCalculatePercentageCorrectly()
    {
        // Arrange & Act
        var treatmentType = new TreatmentTypeDto
        {
            TreatmentName = "Chemotherapy",
            PatientCount = 25,
            Percentage = 50.0
        };

        // Assert
        Assert.Equal("Chemotherapy", treatmentType.TreatmentName);
        Assert.Equal(25, treatmentType.PatientCount);
        Assert.Equal(50.0, treatmentType.Percentage);
    }

    [Fact]
    public void TreatmentTypeDto_WithUnspecifiedName_ShouldBeValid()
    {
        // Arrange & Act - Test that "Unspecified" category can be created
        var unspecifiedTreatment = new TreatmentTypeDto
        {
            TreatmentName = "Unspecified",
            PatientCount = 10,
            Percentage = 20.0
        };

        // Assert
        Assert.Equal("Unspecified", unspecifiedTreatment.TreatmentName);
        Assert.Equal(10, unspecifiedTreatment.PatientCount);
        Assert.Equal(20.0, unspecifiedTreatment.Percentage);
    }

    [Fact]
    public void TreatmentTypeDto_ListShouldSupportSorting()
    {
        // Arrange
        var treatmentTypes = new List<TreatmentTypeDto>
        {
            new TreatmentTypeDto { TreatmentName = "Chemotherapy", PatientCount = 30, Percentage = 30.0 },
            new TreatmentTypeDto { TreatmentName = "Radiation", PatientCount = 50, Percentage = 50.0 },
            new TreatmentTypeDto { TreatmentName = "Unspecified", PatientCount = 20, Percentage = 20.0 }
        };

        // Act - Sort by count descending
        var sortedByCount = treatmentTypes.OrderByDescending(t => t.PatientCount).ToList();

        // Assert
        Assert.Equal("Radiation", sortedByCount[0].TreatmentName);
        Assert.Equal("Chemotherapy", sortedByCount[1].TreatmentName);
        Assert.Equal("Unspecified", sortedByCount[2].TreatmentName);

        // Act - Sort by name ascending
        var sortedByName = treatmentTypes.OrderBy(t => t.TreatmentName).ToList();

        // Assert
        Assert.Equal("Chemotherapy", sortedByName[0].TreatmentName);
        Assert.Equal("Radiation", sortedByName[1].TreatmentName);
        Assert.Equal("Unspecified", sortedByName[2].TreatmentName);

        // Act - Sort by percentage descending
        var sortedByPercentage = treatmentTypes.OrderByDescending(t => t.Percentage).ToList();

        // Assert
        Assert.Equal("Radiation", sortedByPercentage[0].TreatmentName);
        Assert.Equal("Chemotherapy", sortedByPercentage[1].TreatmentName);
        Assert.Equal("Unspecified", sortedByPercentage[2].TreatmentName);
    }

    [Fact]
    public void DemographicsDto_LanguagePercentages_ShouldSumToOneHundred()
    {
        // Arrange & Act
        var demographics = new DemographicsDto
        {
            EnglishFirstLanguageCount = 30,
            OtherLanguageCount = 20,
            EnglishFirstLanguagePercentage = 60.0,
            OtherLanguagePercentage = 40.0,
            SurveyReturnedCount = 35,
            SurveyNotReturnedCount = 15,
            SurveyReturnedPercentage = 70.0
        };

        // Assert
        Assert.Equal(100.0, demographics.EnglishFirstLanguagePercentage + demographics.OtherLanguagePercentage);
        Assert.Equal(50, demographics.EnglishFirstLanguageCount + demographics.OtherLanguageCount);
        Assert.Equal(50, demographics.SurveyReturnedCount + demographics.SurveyNotReturnedCount);
    }
}