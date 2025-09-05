using ClinicTracking.Core.Entities;
using ClinicTracking.Infrastructure.Data;
using ClinicTracking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClinicTracking.Tests;

public class TreatmentRepositoryTests : IDisposable
{
    private readonly ClinicTrackingDbContext _context;
    private readonly TreatmentRepository _repository;

    public TreatmentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ClinicTrackingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new ClinicTrackingDbContext(options);
        _repository = new TreatmentRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnTreatmentsOrderedByName()
    {
        // Arrange
        var treatments = new[]
        {
            new Treatment { Id = Guid.NewGuid(), Name = "Chemotherapy", CreatedBy = "Test" },
            new Treatment { Id = Guid.NewGuid(), Name = "Radiation Therapy", CreatedBy = "Test" },
            new Treatment { Id = Guid.NewGuid(), Name = "Immunotherapy", CreatedBy = "Test" }
        };

        await _context.Treatments.AddRangeAsync(treatments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        var treatmentList = result.ToList();
        Assert.Equal(3, treatmentList.Count);
        Assert.Equal("Chemotherapy", treatmentList[0].Name);
        Assert.Equal("Immunotherapy", treatmentList[1].Name);
        Assert.Equal("Radiation Therapy", treatmentList[2].Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTreatment()
    {
        // Arrange
        var treatment = new Treatment 
        { 
            Id = Guid.NewGuid(), 
            Name = "Test Treatment", 
            Description = "Test Description",
            CreatedBy = "Test" 
        };

        await _context.Treatments.AddAsync(treatment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(treatment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(treatment.Id, result.Id);
        Assert.Equal("Test Treatment", result.Name);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_WithValidName_ShouldReturnTreatment()
    {
        // Arrange
        var treatment = new Treatment 
        { 
            Id = Guid.NewGuid(), 
            Name = "Unique Treatment Name", 
            CreatedBy = "Test" 
        };

        await _context.Treatments.AddAsync(treatment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Unique Treatment Name");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(treatment.Id, result.Id);
        Assert.Equal("Unique Treatment Name", result.Name);
    }

    [Fact]
    public async Task IsInUseAsync_WhenTreatmentIsUsed_ShouldReturnTrue()
    {
        // Arrange
        var treatment = new Treatment 
        { 
            Id = Guid.NewGuid(), 
            Name = "Used Treatment", 
            CreatedBy = "Test" 
        };

        var patient = new PatientTracking
        {
            Id = Guid.NewGuid(),
            MRN = "TEST001",
            Name = "Test Patient",
            ReferralDate = DateTime.Today,
            TreatmentId = treatment.Id,
            CreatedBy = "Test"
        };

        await _context.Treatments.AddAsync(treatment);
        await _context.PatientTrackings.AddAsync(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsInUseAsync(treatment.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsInUseAsync_WhenTreatmentIsNotUsed_ShouldReturnFalse()
    {
        // Arrange
        var treatment = new Treatment 
        { 
            Id = Guid.NewGuid(), 
            Name = "Unused Treatment", 
            CreatedBy = "Test" 
        };

        await _context.Treatments.AddAsync(treatment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsInUseAsync(treatment.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTreatmentToDatabase()
    {
        // Arrange
        var treatment = new Treatment 
        { 
            Id = Guid.NewGuid(), 
            Name = "New Treatment", 
            CreatedBy = "Test" 
        };

        // Act
        await _repository.AddAsync(treatment);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Treatments.FindAsync(treatment.Id);
        Assert.NotNull(result);
        Assert.Equal("New Treatment", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTreatmentInDatabase()
    {
        // Arrange
        var treatment = new Treatment 
        { 
            Id = Guid.NewGuid(), 
            Name = "Original Name", 
            CreatedBy = "Test" 
        };

        await _context.Treatments.AddAsync(treatment);
        await _context.SaveChangesAsync();

        // Act
        treatment.Name = "Updated Name";
        treatment.Description = "Updated Description";
        await _repository.UpdateAsync(treatment);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Treatments.FindAsync(treatment.Id);
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Updated Description", result.Description);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTreatmentFromDatabase()
    {
        // Arrange
        var treatment = new Treatment 
        { 
            Id = Guid.NewGuid(), 
            Name = "To Delete", 
            CreatedBy = "Test" 
        };

        await _context.Treatments.AddAsync(treatment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(treatment.Id);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Treatments.FindAsync(treatment.Id);
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}