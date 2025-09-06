using System.ComponentModel.DataAnnotations;

namespace ClinicTracking.API.DTOs;

public class TreatmentDto
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public bool IsAutoAdded { get; set; }
    
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    
    public bool IsInUse { get; set; }
}