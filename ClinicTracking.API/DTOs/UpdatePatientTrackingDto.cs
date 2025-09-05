using System.ComponentModel.DataAnnotations;

namespace ClinicTracking.API.DTOs;

public class UpdatePatientTrackingDto
{
    [Required]
    [StringLength(50)]
    public string MRN { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    public DateTime ReferralDate { get; set; }

    public DateTime? CounsellingDate { get; set; }

    [StringLength(200)]
    public string? CounsellingBy { get; set; }

    [StringLength(500)]
    public string? DelayReason { get; set; }

    public bool SurveyReturned { get; set; }
    public bool IsEnglishFirstLanguage { get; set; }

    public Guid? TreatmentId { get; set; }

    public bool Adjuvant { get; set; }
    public bool Palliative { get; set; }
    public DateTime? DispensedDate { get; set; }
    public DateTime? ImagingDate { get; set; }
    public DateTime? ResultsDate { get; set; }
    public DateTime? NextCycleDue { get; set; }
    public DateTime? NextAppointment { get; set; }
    public string? Comments { get; set; }
}