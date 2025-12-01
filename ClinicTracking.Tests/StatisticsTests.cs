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

    [Fact]
    public void StatisticsDto_WithNullCollections_ShouldInitializeToEmptyLists()
    {
        // Arrange & Act
        var statistics = new StatisticsDto();

        // Assert - All collections should be initialized to empty lists, not null
        Assert.NotNull(statistics.WaitTimeDistribution);
        Assert.NotNull(statistics.TreatmentTimeDistribution);
        Assert.NotNull(statistics.TreatmentTypes);
        Assert.NotNull(statistics.CounsellorMetrics);
        Assert.NotNull(statistics.ReferralTrends);
        Assert.NotNull(statistics.SummaryMetrics);
        Assert.NotNull(statistics.Demographics);
        Assert.NotNull(statistics.OperationalMetrics);
        
        // Empty lists should be returned
        Assert.Empty(statistics.WaitTimeDistribution);
        Assert.Empty(statistics.TreatmentTimeDistribution);
        Assert.Empty(statistics.TreatmentTypes);
        Assert.Empty(statistics.CounsellorMetrics);
        Assert.Empty(statistics.ReferralTrends);
    }

    [Fact]
    public void TreatmentTypeDto_WithUnspecifiedTreatmentName_ShouldBeValid()
    {
        // Arrange & Act - When treatment is not assigned, "Unspecified" is used
        var treatmentType = new TreatmentTypeDto
        {
            TreatmentName = "Unspecified",
            PatientCount = 10,
            Percentage = 25.0
        };

        // Assert - "Unspecified" is the expected value for null/missing treatments
        Assert.Equal("Unspecified", treatmentType.TreatmentName);
        Assert.Equal(10, treatmentType.PatientCount);
        Assert.Equal(25.0, treatmentType.Percentage);
    }

    [Fact]
    public void CounsellorMetricDto_WithNullAverageWaitTime_ShouldBeValid()
    {
        // Arrange & Act
        var metric = new CounsellorMetricDto
        {
            CounsellorName = "Test Counsellor",
            PatientCount = 5,
            AverageWaitTime = null
        };

        // Assert
        Assert.Equal("Test Counsellor", metric.CounsellorName);
        Assert.Equal(5, metric.PatientCount);
        Assert.Null(metric.AverageWaitTime);
    }

    [Fact]
    public void DemographicsDto_WithDefaultValues_ShouldHaveZeroCounts()
    {
        // Arrange & Act
        var demographics = new DemographicsDto();

        // Assert - All numeric fields should default to 0
        Assert.Equal(0, demographics.EnglishFirstLanguageCount);
        Assert.Equal(0, demographics.OtherLanguageCount);
        Assert.Equal(0, demographics.EnglishFirstLanguagePercentage);
        Assert.Equal(0, demographics.OtherLanguagePercentage);
        Assert.Equal(0, demographics.SurveyReturnedCount);
        Assert.Equal(0, demographics.SurveyNotReturnedCount);
        Assert.Equal(0, demographics.SurveyReturnedPercentage);
    }

    [Fact]
    public void OperationalMetricsDto_WithDefaultValues_ShouldHaveZeroCounts()
    {
        // Arrange & Act
        var metrics = new OperationalMetricsDto();

        // Assert
        Assert.Equal(0, metrics.AwaitingCounsellingCount);
        Assert.Equal(0, metrics.AwaitingTreatmentCount);
        Assert.NotNull(metrics.NextAppointmentDistribution);
        Assert.Empty(metrics.NextAppointmentDistribution);
    }

    [Fact]
    public void SummaryMetricsDto_WithNullNumericFields_ShouldBeValid()
    {
        // Arrange & Act
        var metrics = new SummaryMetricsDto
        {
            TotalPatients = 0,
            AverageWaitTime = null,
            MedianWaitTime = null,
            MaxWaitTime = null,
            MinWaitTime = null,
            AverageTreatmentTime = null,
            MedianTreatmentTime = null,
            MaxTreatmentTime = null,
            MinTreatmentTime = null,
            SurveyCompletionRate = 0,
            PatientsSeenWithinTargetTime = 0
        };

        // Assert - All nullable fields should accept null
        Assert.Null(metrics.AverageWaitTime);
        Assert.Null(metrics.MedianWaitTime);
        Assert.Null(metrics.MaxWaitTime);
        Assert.Null(metrics.MinWaitTime);
        Assert.Null(metrics.AverageTreatmentTime);
        Assert.Null(metrics.MedianTreatmentTime);
        Assert.Null(metrics.MaxTreatmentTime);
        Assert.Null(metrics.MinTreatmentTime);
    }
}