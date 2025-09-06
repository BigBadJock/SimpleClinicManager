using ClinicTracking.API.DTOs;
using OfficeOpenXml;
using System.Text;

namespace ClinicTracking.API.Services;

public interface IExportService
{
    byte[] ExportToCsv(IEnumerable<PatientTrackingDto> patients);
    byte[] ExportToExcel(IEnumerable<PatientTrackingDto> patients);
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

    public byte[] ExportToExcel(IEnumerable<PatientTrackingDto> patients)
    {
        // Set EPPlus license for non-commercial use (suppress warning for demo purposes)
        #pragma warning disable CS0618
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        #pragma warning restore CS0618
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Patients");
        
        // Add headers
        var headers = new[]
        {
            "MRN", "Name", "Referral Date", "Counselling Date", "Counselling By", "Delay Reason",
            "Survey Returned", "English First Language", "Treatment", "Adjuvant", "Palliative",
            "Dispensed Date", "Imaging Date", "Results Date", "Next Cycle Due", "Next Appointment",
            "Comments", "Wait Time (Days)", "Treatment Time (Days)", "Created By", "Created On",
            "Modified By", "Modified On"
        };
        
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }
        
        // Add data
        var patientList = patients.ToList();
        for (int row = 0; row < patientList.Count; row++)
        {
            var patient = patientList[row];
            var excelRow = row + 2; // Excel rows are 1-based, plus header row
            
            worksheet.Cells[excelRow, 1].Value = patient.MRN;
            worksheet.Cells[excelRow, 2].Value = patient.Name;
            worksheet.Cells[excelRow, 3].Value = patient.ReferralDate;
            worksheet.Cells[excelRow, 4].Value = patient.CounsellingDate;
            worksheet.Cells[excelRow, 5].Value = patient.CounsellingBy;
            worksheet.Cells[excelRow, 6].Value = patient.DelayReason;
            worksheet.Cells[excelRow, 7].Value = patient.SurveyReturned ? "Yes" : "No";
            worksheet.Cells[excelRow, 8].Value = patient.IsEnglishFirstLanguage ? "Yes" : "No";
            worksheet.Cells[excelRow, 9].Value = patient.TreatmentName;
            worksheet.Cells[excelRow, 10].Value = patient.Adjuvant ? "Yes" : "No";
            worksheet.Cells[excelRow, 11].Value = patient.Palliative ? "Yes" : "No";
            worksheet.Cells[excelRow, 12].Value = patient.DispensedDate;
            worksheet.Cells[excelRow, 13].Value = patient.ImagingDate;
            worksheet.Cells[excelRow, 14].Value = patient.ResultsDate;
            worksheet.Cells[excelRow, 15].Value = patient.NextCycleDue;
            worksheet.Cells[excelRow, 16].Value = patient.NextAppointment;
            worksheet.Cells[excelRow, 17].Value = patient.Comments;
            worksheet.Cells[excelRow, 18].Value = patient.WaitTimeReferralToCounselling;
            worksheet.Cells[excelRow, 19].Value = patient.TreatTime;
            worksheet.Cells[excelRow, 20].Value = patient.CreatedBy;
            worksheet.Cells[excelRow, 21].Value = patient.CreatedOn;
            worksheet.Cells[excelRow, 22].Value = patient.ModifiedBy;
            worksheet.Cells[excelRow, 23].Value = patient.ModifiedOn;
        }
        
        // Format date columns
        var dateColumns = new[] { 3, 4, 12, 13, 14, 15, 16, 21, 23 };
        foreach (var col in dateColumns)
        {
            worksheet.Column(col).Style.Numberformat.Format = "yyyy-mm-dd";
        }
        
        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        
        return package.GetAsByteArray();
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