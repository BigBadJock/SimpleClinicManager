using System.ComponentModel.DataAnnotations;

namespace ClinicTracking.API.DTOs;

public class ImportResultDto
{
    public int TotalRows { get; set; }
    public int SuccessfulImports { get; set; }
    public int SkippedRows { get; set; }
    public int NewTreatmentsAdded { get; set; }
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> NewTreatmentNames { get; set; } = new();
    public DateTime ImportedAt { get; set; }
    public string ImportedBy { get; set; } = null!;
}

public class ImportLogEntryDto
{
    public int RowNumber { get; set; }
    public string Level { get; set; } = null!; // "Info", "Warning", "Error"
    public string Message { get; set; } = null!;
    public string? Details { get; set; }
}