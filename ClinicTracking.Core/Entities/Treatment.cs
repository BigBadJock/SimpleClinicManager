using System.ComponentModel.DataAnnotations;

namespace ClinicTracking.Core.Entities;

public class Treatment
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    // Audit fields
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    
    // Navigation property for patients using this treatment
    public virtual ICollection<PatientTracking> Patients { get; set; } = new List<PatientTracking>();
}