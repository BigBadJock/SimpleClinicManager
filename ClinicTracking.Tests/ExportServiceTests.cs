using ClinicTracking.API.DTOs;
using ClinicTracking.API.Services;
using Xunit;

namespace ClinicTracking.Tests;

public class ExportServiceTests
{
    [Fact]
    public void ExportToCsv_ShouldReturnValidCsvData()
    {
        // Arrange
        var exportService = new ExportService();
        var patients = new List<PatientTrackingDto>
        {
            new PatientTrackingDto
            {
                Id = Guid.NewGuid(),
                MRN = "MRN001",
                Name = "John Smith",
                ReferralDate = new DateTime(2024, 1, 1),
                CounsellingDate = new DateTime(2024, 1, 10),
                TreatmentName = "Chemotherapy",
                CreatedBy = "TestUser",
                CreatedOn = new DateTime(2024, 1, 1),
                WaitTimeReferralToCounselling = 9,
                TreatTime = 5
            }
        };

        // Act
        var csvData = exportService.ExportToCsv(patients);

        // Assert
        Assert.NotNull(csvData);
        Assert.True(csvData.Length > 0);
        
        var csvContent = System.Text.Encoding.UTF8.GetString(csvData);
        Assert.Contains("MRN,Name,Referral Date", csvContent);
        Assert.Contains("MRN001,John Smith", csvContent);
        Assert.Contains("Chemotherapy", csvContent);
    }

    [Fact]
    public void ExportToCsv_WithSpecialCharacters_ShouldEscapeCorrectly()
    {
        // Arrange
        var exportService = new ExportService();
        var patients = new List<PatientTrackingDto>
        {
            new PatientTrackingDto
            {
                Id = Guid.NewGuid(),
                MRN = "MRN001",
                Name = "Smith, John \"Jr.\"",
                ReferralDate = new DateTime(2024, 1, 1),
                Comments = "Patient has, commas and \"quotes\"",
                CreatedBy = "TestUser",
                CreatedOn = new DateTime(2024, 1, 1)
            }
        };

        // Act
        var csvData = exportService.ExportToCsv(patients);

        // Assert
        var csvContent = System.Text.Encoding.UTF8.GetString(csvData);
        Assert.Contains("\"Smith, John \"\"Jr.\"\"\"", csvContent);
        Assert.Contains("\"Patient has, commas and \"\"quotes\"\"\"", csvContent);
    }
}