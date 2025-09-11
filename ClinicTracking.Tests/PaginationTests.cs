using ClinicTracking.API.DTOs;
using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using ClinicTracking.Infrastructure.Data;
using ClinicTracking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClinicTracking.Tests;

public class PaginationTests : IDisposable
{
    private readonly ClinicTrackingDbContext _context;
    private readonly IPatientRepository _patientRepository;

    public PaginationTests()
    {
        var options = new DbContextOptionsBuilder<ClinicTrackingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ClinicTrackingDbContext(options);
        _patientRepository = new PatientRepository(_context);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageSize()
    {
        // Arrange - Create test patients
        var patients = CreateTestPatients(50);
        foreach (var patient in patients)
        {
            await _patientRepository.AddAsync(patient);
        }
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _patientRepository.GetPagedAsync(1, 25);

        // Assert
        Assert.Equal(25, items.Count());
        Assert.Equal(50, totalCount);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectSecondPage()
    {
        // Arrange - Create test patients
        var patients = CreateTestPatients(30);
        foreach (var patient in patients)
        {
            await _patientRepository.AddAsync(patient);
        }
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _patientRepository.GetPagedAsync(2, 25);

        // Assert
        Assert.Equal(5, items.Count()); // Only 5 items on second page
        Assert.Equal(30, totalCount);
    }

    [Fact]
    public async Task SearchPagedAsync_ReturnsFilteredResults()
    {
        // Arrange - Create test patients with some matching names
        var patients = CreateTestPatients(20);
        patients[0].Name = "John Smith";
        patients[1].Name = "John Doe";
        patients[2].Name = "Jane Smith";

        foreach (var patient in patients)
        {
            await _patientRepository.AddAsync(patient);
        }
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _patientRepository.SearchPagedAsync("John", 1, 10);

        // Assert
        Assert.Equal(2, items.Count()); // Should find 2 Johns
        Assert.Equal(2, totalCount);
        Assert.All(items, p => Assert.Contains("John", p.Name));
    }

    [Fact]
    public async Task GetAwaitingCounsellingPagedAsync_ReturnsOnlyPatientsWithoutCounsellingDate()
    {
        // Arrange
        var patients = CreateTestPatients(10);
        // Set some patients to have counselling date
        for (int i = 0; i < 5; i++)
        {
            patients[i].CounsellingDate = DateTime.Now;
        }

        foreach (var patient in patients)
        {
            await _patientRepository.AddAsync(patient);
        }
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _patientRepository.GetAwaitingCounsellingPagedAsync(1, 10);

        // Assert
        Assert.Equal(5, items.Count()); // Should find 5 patients without counselling date
        Assert.Equal(5, totalCount);
        Assert.All(items, p => Assert.Null(p.CounsellingDate));
    }

    [Fact]
    public void PaginationParameters_LimitsMaxPageSize()
    {
        // Arrange & Act
        var pagination = new PaginationParameters { PageSize = 200 };

        // Assert
        Assert.Equal(100, pagination.PageSize); // Should be capped at 100
    }

    [Fact]
    public void PagedResult_CalculatesPropertiesCorrectly()
    {
        // Arrange
        var items = Enumerable.Range(1, 25);
        
        // Act
        var result = new PagedResult<int>
        {
            Items = items,
            TotalCount = 100,
            PageNumber = 2,
            PageSize = 25
        };

        // Assert
        Assert.Equal(4, result.TotalPages); // 100 / 25 = 4
        Assert.True(result.HasPreviousPage); // Page 2 has previous
        Assert.True(result.HasNextPage); // Page 2 of 4 has next
    }

    private List<PatientTracking> CreateTestPatients(int count)
    {
        var patients = new List<PatientTracking>();
        for (int i = 0; i < count; i++)
        {
            patients.Add(new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = $"MRN{i:000}",
                Name = $"Patient {i}",
                ReferralDate = DateTime.Now.AddDays(-i),
                CreatedBy = "Test",
                CreatedOn = DateTime.UtcNow
            });
        }
        return patients;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}