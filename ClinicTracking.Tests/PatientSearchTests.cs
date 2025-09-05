using ClinicTracking.Core.Entities;
using ClinicTracking.Infrastructure.Data;
using ClinicTracking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClinicTracking.Tests;

public class PatientSearchTests
{
    private ClinicTrackingDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ClinicTrackingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ClinicTrackingDbContext(options);
    }

    private async Task<ClinicTrackingDbContext> SeedTestDataAsync()
    {
        var context = GetInMemoryDbContext();
        
        var patients = new[]
        {
            new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = "MRN001",
                Name = "John Smith",
                ReferralDate = DateTime.Today,
                CreatedBy = "TestUser"
            },
            new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = "MRN002",
                Name = "JANE THOMPSON",
                ReferralDate = DateTime.Today,
                CreatedBy = "TestUser"
            },
            new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = "ABC123",
                Name = "alice smiley",
                ReferralDate = DateTime.Today,
                CreatedBy = "TestUser"
            },
            new PatientTracking
            {
                Id = Guid.NewGuid(),
                MRN = "XYZ789",
                Name = "Bob Williams",
                ReferralDate = DateTime.Today,
                CreatedBy = "TestUser"
            }
        };

        await context.PatientTrackings.AddRangeAsync(patients);
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task SearchAsync_WithCaseInsensitiveName_ShouldReturnMatches()
    {
        // Arrange
        using var context = await SeedTestDataAsync();
        var repository = new PatientRepository(context);

        // Act - search for "john" (lowercase) should match "John Smith"
        var results = await repository.SearchAsync("john");

        // Assert
        Assert.Single(results);
        Assert.Contains(results, p => p.Name == "John Smith");
    }

    [Fact]
    public async Task SearchAsync_WithCaseInsensitiveMRN_ShouldReturnMatches()
    {
        // Arrange
        using var context = await SeedTestDataAsync();
        var repository = new PatientRepository(context);

        // Act - search for "mrn001" (lowercase) should match "MRN001"
        var results = await repository.SearchAsync("mrn001");

        // Assert
        Assert.Single(results);
        Assert.Contains(results, p => p.MRN == "MRN001");
    }

    [Fact]
    public async Task SearchAsync_WithPartialName_ShouldReturnMatches()
    {
        // Arrange
        using var context = await SeedTestDataAsync();
        var repository = new PatientRepository(context);

        // Act - search for "smi" should match both "John Smith" and "alice smiley"
        var results = await repository.SearchAsync("smi");

        // Assert
        Assert.Equal(2, results.Count());
        Assert.Contains(results, p => p.Name == "John Smith");
        Assert.Contains(results, p => p.Name == "alice smiley");
    }

    [Fact]
    public async Task SearchAsync_WithPartialMRN_ShouldReturnMatches()
    {
        // Arrange
        using var context = await SeedTestDataAsync();
        var repository = new PatientRepository(context);

        // Act - search for "123" should match "ABC123"
        var results = await repository.SearchAsync("123");

        // Assert
        Assert.Single(results);
        Assert.Contains(results, p => p.MRN == "ABC123");
    }

    [Fact]
    public async Task SearchAsync_WithUpperCaseSearch_ShouldReturnLowerCaseMatches()
    {
        // Arrange
        using var context = await SeedTestDataAsync();
        var repository = new PatientRepository(context);

        // Act - search for "ALICE" (uppercase) should match "alice smiley" (lowercase)
        var results = await repository.SearchAsync("ALICE");

        // Assert
        Assert.Single(results);
        Assert.Contains(results, p => p.Name == "alice smiley");
    }

    [Fact]
    public async Task SearchAsync_WithMixedCase_ShouldReturnAllMatches()
    {
        // Arrange
        using var context = await SeedTestDataAsync();
        var repository = new PatientRepository(context);

        // Act - search for "JoHn" (mixed case) should match "John Smith"
        var results = await repository.SearchAsync("JoHn");

        // Assert
        Assert.Single(results);
        Assert.Contains(results, p => p.Name == "John Smith");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmpty()
    {
        // Arrange
        using var context = await SeedTestDataAsync();
        var repository = new PatientRepository(context);

        // Act - search for something that doesn't exist
        var results = await repository.SearchAsync("NonExistentPatient");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyString_ShouldReturnEmpty()
    {
        // Arrange
        using var context = await SeedTestDataAsync();
        var repository = new PatientRepository(context);

        // Act
        var results = await repository.SearchAsync("");

        // Assert
        Assert.Empty(results);
    }
}