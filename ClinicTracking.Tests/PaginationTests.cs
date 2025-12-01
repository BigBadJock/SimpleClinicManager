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

    [Fact]
    public async Task GetPagedAsync_WithHideWithoutReferralDate_FiltersPatients()
    {
        // Arrange - Create test patients with some having referral date and some without
        var patientsWithReferral = CreateTestPatients(10); // All have referral date
        var patientsWithoutReferral = new List<PatientTracking>();
        for (int i = 0; i < 5; i++)
        {
            patientsWithoutReferral.Add(new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = $"NORF{i:000}",
                Name = $"No Referral Patient {i}",
                ReferralDate = null, // No referral date
                CreatedBy = "Test",
                CreatedOn = DateTime.UtcNow
            });
        }

        foreach (var patient in patientsWithReferral)
        {
            await _patientRepository.AddAsync(patient);
        }
        foreach (var patient in patientsWithoutReferral)
        {
            await _patientRepository.AddAsync(patient);
        }
        await _context.SaveChangesAsync();

        // Act - Get all patients (filter disabled)
        var (allItems, allTotalCount) = await _patientRepository.GetPagedAsync(1, 100, hideWithoutReferralDate: false);

        // Act - Get only patients with referral date (filter enabled)
        var (filteredItems, filteredTotalCount) = await _patientRepository.GetPagedAsync(1, 100, hideWithoutReferralDate: true);

        // Assert
        Assert.Equal(15, allTotalCount); // All 15 patients
        Assert.Equal(10, filteredTotalCount); // Only 10 patients with referral date
        Assert.All(filteredItems, p => Assert.NotNull(p.ReferralDate));
    }

    [Fact]
    public async Task GetAwaitingCounsellingPagedAsync_WithHideWithoutReferralDate_FiltersPatients()
    {
        // Arrange - Create test patients awaiting counselling
        var patientsWithReferral = new List<PatientTracking>();
        var patientsWithoutReferral = new List<PatientTracking>();
        
        for (int i = 0; i < 5; i++)
        {
            patientsWithReferral.Add(new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = $"WRF{i:000}",
                Name = $"With Referral {i}",
                ReferralDate = DateTime.Now.AddDays(-i),
                CounsellingDate = null, // Awaiting counselling
                CreatedBy = "Test",
                CreatedOn = DateTime.UtcNow
            });
            patientsWithoutReferral.Add(new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = $"NRFC{i:000}",
                Name = $"No Referral {i}",
                ReferralDate = null, // No referral date
                CounsellingDate = null, // Awaiting counselling
                CreatedBy = "Test",
                CreatedOn = DateTime.UtcNow
            });
        }

        foreach (var patient in patientsWithReferral.Concat(patientsWithoutReferral))
        {
            await _patientRepository.AddAsync(patient);
        }
        await _context.SaveChangesAsync();

        // Act - Get all awaiting counselling (filter disabled)
        var (allItems, allTotalCount) = await _patientRepository.GetAwaitingCounsellingPagedAsync(1, 100, hideWithoutReferralDate: false);

        // Act - Get only awaiting counselling with referral date (filter enabled)
        var (filteredItems, filteredTotalCount) = await _patientRepository.GetAwaitingCounsellingPagedAsync(1, 100, hideWithoutReferralDate: true);

        // Assert
        Assert.Equal(10, allTotalCount); // All 10 patients awaiting counselling
        Assert.Equal(5, filteredTotalCount); // Only 5 patients with referral date
        Assert.All(filteredItems, p => Assert.NotNull(p.ReferralDate));
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