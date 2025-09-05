namespace ClinicTracking.Client.DTOs;

public class PatientTrackingDto
{
    public Guid Id { get; set; }
    public string MRN { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime ReferralDate { get; set; }
    public DateTime? CounsellingDate { get; set; }
    public string? DelayReason { get; set; }
    public bool SurveyReturned { get; set; }
    public bool IsEnglishFirstLanguage { get; set; } = true;
    public string? Treatment { get; set; }
    public bool Adjuvant { get; set; }
    public bool Palliative { get; set; }
    public DateTime? DispensedDate { get; set; }
    public DateTime? ImagingDate { get; set; }
    public DateTime? ResultsDate { get; set; }
    public DateTime? NextCycleDue { get; set; }
    public DateTime? NextAppointment { get; set; }
    public string? Comments { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }

    // Calculated fields
    public int? WaitTimeReferralToCounselling { get; set; }
    public int? TreatTime { get; set; }
}