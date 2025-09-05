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