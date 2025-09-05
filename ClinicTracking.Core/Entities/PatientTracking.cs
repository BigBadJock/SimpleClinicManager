namespace ClinicTracking.Core.Entities;

public class PatientTracking
{
    public Guid Id { get; set; }
    public string MRN { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime ReferralDate { get; set; }
    public DateTime? CounsellingDate { get; set; }
    public string? CounsellingBy { get; set; }
    public string? DelayReason { get; set; }
    public bool SurveyReturned { get; set; }
    public bool IsEnglishFirstLanguage { get; set; } = true;
    public Guid? TreatmentId { get; set; }
    public virtual Treatment? TreatmentLookup { get; set; }
    public bool Adjuvant { get; set; }
    public bool Palliative { get; set; }
    public DateTime? DispensedDate { get; set; }
    public DateTime? ImagingDate { get; set; }
    public DateTime? ResultsDate { get; set; }
    public DateTime? NextCycleDue { get; set; }
    public DateTime? NextAppointment { get; set; }
    public string? Comments { get; set; }

    // Audit fields
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }

    // Calculated fields (not mapped to DB)
    public int? WaitTimeReferralToCounselling =>
        CounsellingDate.HasValue ? (int?)(CounsellingDate.Value - ReferralDate).TotalDays : null;

    public int? TreatTime =>
        (CounsellingDate.HasValue && DispensedDate.HasValue)
            ? (int?)(DispensedDate.Value - CounsellingDate.Value).TotalDays
            : null;
}