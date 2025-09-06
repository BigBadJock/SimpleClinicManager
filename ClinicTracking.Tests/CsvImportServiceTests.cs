using ClinicTracking.API.DTOs;
using ClinicTracking.API.Services;
using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace ClinicTracking.Tests;

public class CsvImportServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<ImportService>> _mockLogger;
    private readonly Mock<IPatientRepository> _mockPatientRepo;
    private readonly Mock<ITreatmentRepository> _mockTreatmentRepo;
    private readonly ImportService _importService;

    public CsvImportServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<ImportService>>();
        _mockPatientRepo = new Mock<IPatientRepository>();
        _mockTreatmentRepo = new Mock<ITreatmentRepository>();
        
        _mockUnitOfWork.Setup(u => u.Patients).Returns(_mockPatientRepo.Object);
        _mockUnitOfWork.Setup(u => u.Treatments).Returns(_mockTreatmentRepo.Object);
        
        _importService = new ImportService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ValidateCsvFileAsync_WithValidCsv_ReturnsTrue()
    {
        // Arrange
        var csvContent = "Name,MRN,Treatment\nJohn Smith,MRN001,Chemotherapy";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _importService.ValidateCsvFileAsync(stream);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateCsvFileAsync_WithMissingRequiredHeaders_ReturnsFalse()
    {
        // Arrange
        var csvContent = "Treatment,Adjuvant\nChemotherapy,true";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _importService.ValidateCsvFileAsync(stream);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithValidData_ReturnsSuccessfulResult()
    {
        // Arrange
        var csvContent = @"Name,MRN,Treatment,Survey,english 1st language
John Smith,MRN001,Chemotherapy,true,yes
Jane Doe,MRN002,Radiation,false,no";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        _mockPatientRepo.Setup(r => r.GetByMRNAsync(It.IsAny<string>()))
            .ReturnsAsync((PatientTracking?)null);
        
        var existingTreatment = new Treatment { Id = Guid.NewGuid(), Name = "Chemotherapy" };
        _mockTreatmentRepo.Setup(r => r.GetByNameAsync("Chemotherapy"))
            .ReturnsAsync(existingTreatment);
        _mockTreatmentRepo.Setup(r => r.GetByNameAsync("Radiation"))
            .ReturnsAsync((Treatment?)null);

        // Act
        var result = await _importService.ImportFromCsvAsync(stream, "test.csv", "TestUser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalRows);
        Assert.Equal(2, result.SuccessfulImports);
        Assert.Equal(0, result.SkippedRows);
        Assert.Equal(1, result.NewTreatmentsAdded);
        Assert.Contains("Radiation", result.NewTreatmentNames);
        Assert.Equal("TestUser", result.ImportedBy);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithQuotedFields_ParsesCorrectly()
    {
        // Arrange
        var csvContent = @"Name,MRN,comments
""Smith, John"",MRN001,""Patient has, commas and ""quotes""""""";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        _mockPatientRepo.Setup(r => r.GetByMRNAsync(It.IsAny<string>()))
            .ReturnsAsync((PatientTracking?)null);

        // Act
        var result = await _importService.ImportFromCsvAsync(stream, "test.csv", "TestUser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalRows);
        Assert.Equal(1, result.SuccessfulImports);
        Assert.Equal(0, result.SkippedRows);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithInvalidBooleanValues_MovesToComments()
    {
        // Arrange
        var csvContent = @"Name,MRN,Survey,english 1st language
John Smith,MRN001,maybe,sometimes";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        _mockPatientRepo.Setup(r => r.GetByMRNAsync(It.IsAny<string>()))
            .ReturnsAsync((PatientTracking?)null);

        PatientTracking? capturedPatient = null;
        _mockPatientRepo.Setup(r => r.AddAsync(It.IsAny<PatientTracking>()))
            .Callback<PatientTracking>(p => capturedPatient = p);

        // Act
        var result = await _importService.ImportFromCsvAsync(stream, "test.csv", "TestUser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.SuccessfulImports);
        Assert.NotNull(capturedPatient);
        Assert.Contains("Invalid boolean value 'maybe' for survey", capturedPatient.Comments);
        Assert.Contains("Invalid boolean value 'sometimes' for english1stlanguage", capturedPatient.Comments);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithInvalidDateValues_MovesToComments()
    {
        // Arrange
        var csvContent = @"Name,MRN,Referral date,Counselling date
John Smith,MRN001,not-a-date,2024-13-45";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        _mockPatientRepo.Setup(r => r.GetByMRNAsync(It.IsAny<string>()))
            .ReturnsAsync((PatientTracking?)null);

        PatientTracking? capturedPatient = null;
        _mockPatientRepo.Setup(r => r.AddAsync(It.IsAny<PatientTracking>()))
            .Callback<PatientTracking>(p => capturedPatient = p);

        // Act
        var result = await _importService.ImportFromCsvAsync(stream, "test.csv", "TestUser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.SuccessfulImports);
        Assert.NotNull(capturedPatient);
        Assert.Contains("Invalid date value 'not-a-date' for referraldate", capturedPatient.Comments);
        Assert.Contains("Invalid date value '2024-13-45' for counsellingdate", capturedPatient.Comments);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithMissingRequiredFields_SkipsRow()
    {
        // Arrange
        var csvContent = @"Name,MRN,Treatment
John Smith,MRN001,Chemotherapy
,MRN002,Radiation
Jane Doe,,Surgery";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        _mockPatientRepo.Setup(r => r.GetByMRNAsync(It.IsAny<string>()))
            .ReturnsAsync((PatientTracking?)null);

        // Act
        var result = await _importService.ImportFromCsvAsync(stream, "test.csv", "TestUser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalRows);
        Assert.Equal(1, result.SuccessfulImports);
        Assert.Equal(2, result.SkippedRows);
        Assert.Equal(2, result.Warnings.Count);
        Assert.Contains("Row 3: Skipped - Missing required MRN or Name", result.Warnings);
        Assert.Contains("Row 4: Skipped - Missing required MRN or Name", result.Warnings);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithDuplicateMRN_SkipsRow()
    {
        // Arrange
        var csvContent = @"Name,MRN,Treatment
John Smith,MRN001,Chemotherapy";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        var existingPatient = new PatientTracking { Id = Guid.NewGuid(), MRN = "MRN001", Name = "Existing Patient" };
        _mockPatientRepo.Setup(r => r.GetByMRNAsync("MRN001"))
            .ReturnsAsync(existingPatient);

        // Act
        var result = await _importService.ImportFromCsvAsync(stream, "test.csv", "TestUser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalRows);
        Assert.Equal(0, result.SuccessfulImports);
        Assert.Equal(1, result.SkippedRows);
        Assert.Contains("Row 2: Skipped - Patient with MRN 'MRN001' already exists in database", result.Warnings);
    }

    [Fact]
    public async Task ImportFromCsvAsync_WithNewTreatment_CreatesAutoAddedTreatment()
    {
        // Arrange
        var csvContent = @"Name,MRN,Treatment
John Smith,MRN001,New Experimental Treatment";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        _mockPatientRepo.Setup(r => r.GetByMRNAsync(It.IsAny<string>()))
            .ReturnsAsync((PatientTracking?)null);
        _mockTreatmentRepo.Setup(r => r.GetByNameAsync("New Experimental Treatment"))
            .ReturnsAsync((Treatment?)null);

        Treatment? capturedTreatment = null;
        _mockTreatmentRepo.Setup(r => r.AddAsync(It.IsAny<Treatment>()))
            .Callback<Treatment>(t => capturedTreatment = t);

        PatientTracking? capturedPatient = null;
        _mockPatientRepo.Setup(r => r.AddAsync(It.IsAny<PatientTracking>()))
            .Callback<PatientTracking>(p => capturedPatient = p);

        // Act
        var result = await _importService.ImportFromCsvAsync(stream, "test.csv", "TestUser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.SuccessfulImports);
        Assert.Equal(1, result.NewTreatmentsAdded);
        Assert.Contains("New Experimental Treatment", result.NewTreatmentNames);
        
        Assert.NotNull(capturedTreatment);
        Assert.Equal("New Experimental Treatment", capturedTreatment.Name);
        Assert.True(capturedTreatment.IsAutoAdded);
        Assert.Equal("import", capturedTreatment.CreatedBy);
        
        Assert.NotNull(capturedPatient);
        Assert.Contains("New treatment auto-added during import: New Experimental Treatment", capturedPatient.Comments);
    }
}