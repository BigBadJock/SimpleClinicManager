namespace ClinicTracking.API.DTOs;

public class StatisticsDto
{
    public SummaryMetricsDto SummaryMetrics { get; set; } = new();
    public List<TimeDistributionDto> WaitTimeDistribution { get; set; } = new();
    public List<TimeDistributionDto> TreatmentTimeDistribution { get; set; } = new();
    public List<TreatmentTypeDto> TreatmentTypes { get; set; } = new();
    public List<CounsellorMetricDto> CounsellorMetrics { get; set; } = new();
    public DemographicsDto Demographics { get; set; } = new();
    public OperationalMetricsDto OperationalMetrics { get; set; } = new();
    public List<TrendDataDto> ReferralTrends { get; set; } = new();
}

public class SummaryMetricsDto
{
    public int TotalPatients { get; set; }
    public double? AverageWaitTime { get; set; }
    public double? MedianWaitTime { get; set; }
    public int? MaxWaitTime { get; set; }
    public int? MinWaitTime { get; set; }
    public double? AverageTreatmentTime { get; set; }
    public double? MedianTreatmentTime { get; set; }
    public int? MaxTreatmentTime { get; set; }
    public int? MinTreatmentTime { get; set; }
    public double SurveyCompletionRate { get; set; }
    public double PatientsSeenWithinTargetTime { get; set; }
    public int TargetDays { get; set; } = 5;
}

public class TimeDistributionDto
{
    public string Range { get; set; } = null!;
    public int Count { get; set; }
    public int MinDays { get; set; }
    public int MaxDays { get; set; }
}

public class TreatmentTypeDto
{
    public string TreatmentName { get; set; } = null!;
    public int PatientCount { get; set; }
    public double Percentage { get; set; }
}

public class CounsellorMetricDto
{
    public string CounsellorName { get; set; } = null!;
    public int PatientCount { get; set; }
    public double? AverageWaitTime { get; set; }
}

public class DemographicsDto
{
    public int EnglishFirstLanguageCount { get; set; }
    public int OtherLanguageCount { get; set; }
    public double EnglishFirstLanguagePercentage { get; set; }
    public double OtherLanguagePercentage { get; set; }
    public int SurveyReturnedCount { get; set; }
    public int SurveyNotReturnedCount { get; set; }
    public double SurveyReturnedPercentage { get; set; }
}

public class OperationalMetricsDto
{
    public int AwaitingCounsellingCount { get; set; }
    public int AwaitingTreatmentCount { get; set; }
    public List<NextAppointmentDistributionDto> NextAppointmentDistribution { get; set; } = new();
}

public class NextAppointmentDistributionDto
{
    public string DaysRange { get; set; } = null!;
    public int Count { get; set; }
}

public class TrendDataDto
{
    public string Period { get; set; } = null!; // "2024-01", "Week 1", etc.
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class StatisticsFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}