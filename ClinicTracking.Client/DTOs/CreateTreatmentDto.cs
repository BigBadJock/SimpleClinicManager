using System.ComponentModel.DataAnnotations;

namespace ClinicTracking.Client.DTOs;

public class CreateTreatmentDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    [StringLength(500)]
    public string? Description { get; set; }
}