using ClinicTracking.API.DTOs;
using System.Text;

namespace ClinicTracking.API.Services;

public interface IExportService
{
    byte[] ExportToCsv(IEnumerable<PatientTrackingDto> patients);
}

public class ExportService : IExportService
{
    public byte[] ExportToCsv(IEnumerable<PatientTrackingDto> patients)
    {
        var csv = new StringBuilder();
        
        // Add header
        csv.AppendLine("MRN,Name,Referral Date,Counselling Date,Counselling By,Delay Reason,Survey Returned,English First Language,Treatment,Adjuvant,Palliative,Dispensed Date,Imaging Date,Results Date,Next Cycle Due,Next Appointment,Comments,Wait Time (Days),Treatment Time (Days),Created By,Created On,Modified By,Modified On");
        
        // Add data rows
        foreach (var patient in patients)
        {
            csv.AppendLine($"{EscapeCsvField(patient.MRN)}," +
                          $"{EscapeCsvField(patient.Name)}," +
                          $"{patient.ReferralDate:yyyy-MM-dd}," +
                          $"{(patient.CounsellingDate?.ToString("yyyy-MM-dd") ?? "")}," +
                          $"{EscapeCsvField(patient.CounsellingBy ?? "")}," +
                          $"{EscapeCsvField(patient.DelayReason ?? "")}," +
                          $"{(patient.SurveyReturned ? "Yes" : "No")}," +
                          $"{(patient.IsEnglishFirstLanguage ? "Yes" : "No")}," +
                          $"{EscapeCsvField(patient.TreatmentName ?? "")}," +
                          $"{(patient.Adjuvant ? "Yes" : "No")}," +
                          $"{(patient.Palliative ? "Yes" : "No")}," +
                          $"{(patient.DispensedDate?.ToString("yyyy-MM-dd") ?? "")}," +
                          $"{(patient.ImagingDate?.ToString("yyyy-MM-dd") ?? "")}," +
                          $"{(patient.ResultsDate?.ToString("yyyy-MM-dd") ?? "")}," +
                          $"{(patient.NextCycleDue?.ToString("yyyy-MM-dd") ?? "")}," +
                          $"{(patient.NextAppointment?.ToString("yyyy-MM-dd") ?? "")}," +
                          $"{EscapeCsvField(patient.Comments ?? "")}," +
                          $"{(patient.WaitTimeReferralToCounselling?.ToString() ?? "")}," +
                          $"{(patient.TreatTime?.ToString() ?? "")}," +
                          $"{EscapeCsvField(patient.CreatedBy)}," +
                          $"{patient.CreatedOn:yyyy-MM-dd HH:mm:ss}," +
                          $"{EscapeCsvField(patient.ModifiedBy ?? "")}," +
                          $"{(patient.ModifiedOn?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")}");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }
    
    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "";
            
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        
        return field;
    }
}