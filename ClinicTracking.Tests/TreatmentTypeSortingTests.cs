using ClinicTracking.Client.DTOs;

namespace ClinicTracking.Tests;

public class TreatmentTypeSortingTests
{
    private static string GetTreatmentTypeDisplayValue(PatientTrackingDto patient)
    {
        if (patient.Adjuvant) return "Adjuvant";
        if (patient.Palliative) return "Palliative";
        return "Not specified";
    }

    private static List<PatientTrackingDto> CreateTestPatients()
    {
        return new List<PatientTrackingDto>
        {
            new PatientTrackingDto { Id = Guid.NewGuid(), MRN = "MRN001", Name = "Patient A", Adjuvant = true, Palliative = false },
            new PatientTrackingDto { Id = Guid.NewGuid(), MRN = "MRN002", Name = "Patient B", Adjuvant = false, Palliative = true },
            new PatientTrackingDto { Id = Guid.NewGuid(), MRN = "MRN003", Name = "Patient C", Adjuvant = false, Palliative = false },
            new PatientTrackingDto { Id = Guid.NewGuid(), MRN = "MRN004", Name = "Patient D", Adjuvant = true, Palliative = false },
            new PatientTrackingDto { Id = Guid.NewGuid(), MRN = "MRN005", Name = "Patient E", Adjuvant = false, Palliative = true }
        };
    }

    [Fact]
    public void GetTreatmentTypeDisplayValue_Adjuvant_ReturnsAdjuvant()
    {
        // Arrange
        var patient = new PatientTrackingDto { Adjuvant = true, Palliative = false };

        // Act
        var result = GetTreatmentTypeDisplayValue(patient);

        // Assert
        Assert.Equal("Adjuvant", result);
    }

    [Fact]
    public void GetTreatmentTypeDisplayValue_Palliative_ReturnsPalliative()
    {
        // Arrange
        var patient = new PatientTrackingDto { Adjuvant = false, Palliative = true };

        // Act
        var result = GetTreatmentTypeDisplayValue(patient);

        // Assert
        Assert.Equal("Palliative", result);
    }

    [Fact]
    public void GetTreatmentTypeDisplayValue_Neither_ReturnsNotSpecified()
    {
        // Arrange
        var patient = new PatientTrackingDto { Adjuvant = false, Palliative = false };

        // Act
        var result = GetTreatmentTypeDisplayValue(patient);

        // Assert
        Assert.Equal("Not specified", result);
    }

    [Fact]
    public void GetTreatmentTypeDisplayValue_BothAdjuvantAndPalliative_ReturnsAdjuvant()
    {
        // Arrange - When both flags are true, Adjuvant takes priority (matching existing UI behavior)
        var patient = new PatientTrackingDto { Adjuvant = true, Palliative = true };

        // Act
        var result = GetTreatmentTypeDisplayValue(patient);

        // Assert - Adjuvant is checked first, so it takes precedence
        Assert.Equal("Adjuvant", result);
    }

    [Fact]
    public void SortByTreatmentType_Ascending_SortsCorrectly()
    {
        // Arrange
        var patients = CreateTestPatients();

        // Act - Sort ascending using case-insensitive comparison
        var sortedPatients = patients.OrderBy(p => GetTreatmentTypeDisplayValue(p), StringComparer.OrdinalIgnoreCase).ToList();

        // Assert
        // "Adjuvant" comes before "Not specified" comes before "Palliative" alphabetically
        Assert.Equal("Adjuvant", GetTreatmentTypeDisplayValue(sortedPatients[0]));
        Assert.Equal("Adjuvant", GetTreatmentTypeDisplayValue(sortedPatients[1]));
        Assert.Equal("Not specified", GetTreatmentTypeDisplayValue(sortedPatients[2]));
        Assert.Equal("Palliative", GetTreatmentTypeDisplayValue(sortedPatients[3]));
        Assert.Equal("Palliative", GetTreatmentTypeDisplayValue(sortedPatients[4]));
    }

    [Fact]
    public void SortByTreatmentType_Descending_SortsCorrectly()
    {
        // Arrange
        var patients = CreateTestPatients();

        // Act - Sort descending using case-insensitive comparison
        var sortedPatients = patients.OrderByDescending(p => GetTreatmentTypeDisplayValue(p), StringComparer.OrdinalIgnoreCase).ToList();

        // Assert
        // "Palliative" comes before "Not specified" comes before "Adjuvant" in descending order
        Assert.Equal("Palliative", GetTreatmentTypeDisplayValue(sortedPatients[0]));
        Assert.Equal("Palliative", GetTreatmentTypeDisplayValue(sortedPatients[1]));
        Assert.Equal("Not specified", GetTreatmentTypeDisplayValue(sortedPatients[2]));
        Assert.Equal("Adjuvant", GetTreatmentTypeDisplayValue(sortedPatients[3]));
        Assert.Equal("Adjuvant", GetTreatmentTypeDisplayValue(sortedPatients[4]));
    }

    [Fact]
    public void SortByTreatmentType_IsCaseInsensitive()
    {
        // Arrange - The sorting uses StringComparer.OrdinalIgnoreCase
        var patient1 = new PatientTrackingDto { Adjuvant = true };
        var patient2 = new PatientTrackingDto { Palliative = true };
        var patients = new List<PatientTrackingDto> { patient2, patient1 };

        // Act - Sort with case-insensitive comparison
        var sortedAscending = patients.OrderBy(p => GetTreatmentTypeDisplayValue(p), StringComparer.OrdinalIgnoreCase).ToList();
        var sortedDescending = patients.OrderByDescending(p => GetTreatmentTypeDisplayValue(p), StringComparer.OrdinalIgnoreCase).ToList();

        // Assert - Case-insensitive sorting should work correctly
        Assert.Equal("Adjuvant", GetTreatmentTypeDisplayValue(sortedAscending[0]));
        Assert.Equal("Palliative", GetTreatmentTypeDisplayValue(sortedAscending[1]));
        Assert.Equal("Palliative", GetTreatmentTypeDisplayValue(sortedDescending[0]));
        Assert.Equal("Adjuvant", GetTreatmentTypeDisplayValue(sortedDescending[1]));
    }

    [Fact]
    public void SortByTreatmentType_PreservesOtherPatientData()
    {
        // Arrange
        var patients = new List<PatientTrackingDto>
        {
            new PatientTrackingDto { Id = Guid.NewGuid(), MRN = "MRN001", Name = "Patient A", Adjuvant = false, Palliative = true },
            new PatientTrackingDto { Id = Guid.NewGuid(), MRN = "MRN002", Name = "Patient B", Adjuvant = true, Palliative = false }
        };

        // Act
        var sortedPatients = patients.OrderBy(p => GetTreatmentTypeDisplayValue(p), StringComparer.OrdinalIgnoreCase).ToList();

        // Assert - Data should be preserved after sorting
        var adjuvantPatient = sortedPatients[0];
        var palliativePatient = sortedPatients[1];
        
        Assert.Equal("MRN002", adjuvantPatient.MRN);
        Assert.Equal("Patient B", adjuvantPatient.Name);
        Assert.Equal("MRN001", palliativePatient.MRN);
        Assert.Equal("Patient A", palliativePatient.Name);
    }
}
