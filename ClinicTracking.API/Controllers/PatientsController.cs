using ClinicTracking.API.DTOs;
using ClinicTracking.API.Services;
using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicTracking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PatientsController> _logger;
    private readonly IExportService _exportService;

    public PatientsController(IUnitOfWork unitOfWork, ILogger<PatientsController> logger, IExportService exportService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _exportService = exportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientTrackingDto>>> GetAll()
    {
        try
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            var patientDtos = patients.Select(MapToDto);
            return Ok(patientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all patients");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<PatientTrackingDto>>> GetPaged([FromQuery] PaginationParameters pagination)
    {
        try
        {
            var (patients, totalCount) = await _unitOfWork.Patients.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            var patientDtos = patients.Select(MapToDto);
            
            var result = new PagedResult<PatientTrackingDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged patients");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientTrackingDto>> GetById(Guid id)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(patient));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID: {PatientId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("mrn/{mrn}")]
    public async Task<ActionResult<PatientTrackingDto>> GetByMRN(string mrn)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByMRNAsync(mrn);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(patient));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with MRN: {MRN}", mrn);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("awaiting-counselling")]
    public async Task<ActionResult<IEnumerable<PatientTrackingDto>>> GetAwaitingCounselling()
    {
        try
        {
            var patients = await _unitOfWork.Patients.GetAwaitingCounsellingAsync();
            var patientDtos = patients.Select(MapToDto);
            return Ok(patientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients awaiting counselling");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("awaiting-counselling/paged")]
    public async Task<ActionResult<PagedResult<PatientTrackingDto>>> GetAwaitingCounsellingPaged([FromQuery] PaginationParameters pagination)
    {
        try
        {
            var (patients, totalCount) = await _unitOfWork.Patients.GetAwaitingCounsellingPagedAsync(pagination.PageNumber, pagination.PageSize);
            var patientDtos = patients.Select(MapToDto);
            
            var result = new PagedResult<PatientTrackingDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged patients awaiting counselling");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("awaiting-treatment")]
    public async Task<ActionResult<IEnumerable<PatientTrackingDto>>> GetAwaitingTreatment()
    {
        try
        {
            var patients = await _unitOfWork.Patients.GetAwaitingTreatmentAsync();
            var patientDtos = patients.Select(MapToDto);
            return Ok(patientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients awaiting treatment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("awaiting-treatment/paged")]
    public async Task<ActionResult<PagedResult<PatientTrackingDto>>> GetAwaitingTreatmentPaged([FromQuery] PaginationParameters pagination)
    {
        try
        {
            var (patients, totalCount) = await _unitOfWork.Patients.GetAwaitingTreatmentPagedAsync(pagination.PageNumber, pagination.PageSize);
            var patientDtos = patients.Select(MapToDto);
            
            var result = new PagedResult<PatientTrackingDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged patients awaiting treatment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("follow-up-due")]
    public async Task<ActionResult<IEnumerable<PatientTrackingDto>>> GetFollowUpDue()
    {
        try
        {
            var patients = await _unitOfWork.Patients.GetFollowUpDueAsync();
            var patientDtos = patients.Select(MapToDto);
            return Ok(patientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients with follow-up due");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("follow-up-due/paged")]
    public async Task<ActionResult<PagedResult<PatientTrackingDto>>> GetFollowUpDuePaged([FromQuery] PaginationParameters pagination)
    {
        try
        {
            var (patients, totalCount) = await _unitOfWork.Patients.GetFollowUpDuePagedAsync(pagination.PageNumber, pagination.PageSize);
            var patientDtos = patients.Select(MapToDto);
            
            var result = new PagedResult<PatientTrackingDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged patients with follow-up due");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("search/{searchTerm}")]
    public async Task<ActionResult<IEnumerable<PatientTrackingDto>>> Search(string searchTerm)
    {
        try
        {
            var patients = await _unitOfWork.Patients.SearchAsync(searchTerm);
            var patientDtos = patients.Select(MapToDto);
            return Ok(patientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients with term: {SearchTerm}", searchTerm);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("search/{searchTerm}/paged")]
    public async Task<ActionResult<PagedResult<PatientTrackingDto>>> SearchPaged(string searchTerm, [FromQuery] PaginationParameters pagination)
    {
        try
        {
            var (patients, totalCount) = await _unitOfWork.Patients.SearchPagedAsync(searchTerm, pagination.PageNumber, pagination.PageSize);
            var patientDtos = patients.Select(MapToDto);
            
            var result = new PagedResult<PatientTrackingDto>
            {
                Items = patientDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching paged patients with term: {SearchTerm}", searchTerm);
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportCsv([FromQuery] string? filter = null, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var patients = await GetPatientsForExport(filter, searchTerm);
            var csvData = _exportService.ExportToCsv(patients);
            
            var fileName = $"patients_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvData, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting patients to CSV with filter: {Filter}, searchTerm: {SearchTerm}", filter, searchTerm);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("statistics")]
    public async Task<ActionResult<StatisticsDto>> GetStatistics([FromBody] StatisticsFilterDto filter)
    {
        try
        {
            var allPatients = await _unitOfWork.Patients.GetAllAsync();

            // Apply date filtering
            var filteredPatients = allPatients.AsQueryable();
            if (filter.StartDate.HasValue)
            {
                filteredPatients = filteredPatients.Where(p => p.CounsellingDate >= filter.StartDate.Value);
            }
            if (filter.EndDate.HasValue)
            {
                filteredPatients = filteredPatients.Where(p => p.CounsellingDate <= filter.EndDate.Value);
            }

            var patientsList = filteredPatients.ToList();

            var statistics = CalculateStatistics(patientsList);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating statistics");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpPost]
    public async Task<ActionResult<PatientTrackingDto>> Create([FromBody] CreatePatientTrackingDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if MRN already exists
            var existingPatient = await _unitOfWork.Patients.GetByMRNAsync(createDto.MRN);
            if (existingPatient != null)
            {
                return Conflict("A patient with this MRN already exists");
            }

            var patient = MapFromCreateDto(createDto);
            patient.Id = Guid.NewGuid();
            patient.CreatedBy = GetCurrentUser();
            patient.CreatedOn = DateTime.UtcNow;

            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, MapToDto(patient));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PatientTrackingDto>> Update(Guid id, [FromBody] UpdatePatientTrackingDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPatient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (existingPatient == null)
            {
                return NotFound();
            }

            // Check if MRN is being changed and if it conflicts with another patient
            if (existingPatient.MRN != updateDto.MRN)
            {
                var conflictingPatient = await _unitOfWork.Patients.GetByMRNAsync(updateDto.MRN);
                if (conflictingPatient != null && conflictingPatient.Id != id)
                {
                    return Conflict("A patient with this MRN already exists");
                }
            }

            UpdatePatientFromDto(existingPatient, updateDto);
            existingPatient.ModifiedBy = GetCurrentUser();
            existingPatient.ModifiedOn = DateTime.UtcNow;

            await _unitOfWork.Patients.UpdateAsync(existingPatient);
            await _unitOfWork.SaveChangesAsync();

            return Ok(MapToDto(existingPatient));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient with ID: {PatientId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            await _unitOfWork.Patients.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting patient with ID: {PatientId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private PatientTrackingDto MapToDto(PatientTracking patient)
    {
        return new PatientTrackingDto
        {
            Id = patient.Id,
            MRN = patient.MRN,
            Name = patient.Name,
            ReferralDate = patient.ReferralDate,
            CounsellingDate = patient.CounsellingDate,
            CounsellingBy = patient.CounsellingBy,
            DelayReason = patient.DelayReason,
            SurveyReturned = patient.SurveyReturned,
            IsEnglishFirstLanguage = patient.IsEnglishFirstLanguage,
            TreatmentId = patient.TreatmentId,
            TreatmentName = patient.TreatmentLookup?.Name,
            Adjuvant = patient.Adjuvant,
            Palliative = patient.Palliative,
            DispensedDate = patient.DispensedDate,
            ImagingDate = patient.ImagingDate,
            ResultsDate = patient.ResultsDate,
            NextCycleDue = patient.NextCycleDue,
            NextAppointment = patient.NextAppointment,
            Comments = patient.Comments,
            CreatedBy = patient.CreatedBy,
            CreatedOn = patient.CreatedOn,
            ModifiedBy = patient.ModifiedBy,
            ModifiedOn = patient.ModifiedOn,
            WaitTimeReferralToCounselling = patient.WaitTimeReferralToCounselling,
            TreatTime = patient.TreatTime
        };
    }

    private PatientTracking MapFromCreateDto(CreatePatientTrackingDto dto)
    {
        return new PatientTracking
        {
            MRN = dto.MRN,
            Name = dto.Name,
            ReferralDate = dto.ReferralDate,
            CounsellingDate = dto.CounsellingDate,
            CounsellingBy = dto.CounsellingBy,
            DelayReason = dto.DelayReason,
            SurveyReturned = dto.SurveyReturned,
            IsEnglishFirstLanguage = dto.IsEnglishFirstLanguage,
            TreatmentId = dto.TreatmentId,
            Adjuvant = dto.Adjuvant,
            Palliative = dto.Palliative,
            DispensedDate = dto.DispensedDate,
            ImagingDate = dto.ImagingDate,
            ResultsDate = dto.ResultsDate,
            NextCycleDue = dto.NextCycleDue,
            NextAppointment = dto.NextAppointment,
            Comments = dto.Comments
        };
    }

    private void UpdatePatientFromDto(PatientTracking patient, UpdatePatientTrackingDto dto)
    {
        patient.MRN = dto.MRN;
        patient.Name = dto.Name;
        patient.ReferralDate = dto.ReferralDate;
        patient.CounsellingDate = dto.CounsellingDate;
        patient.CounsellingBy = dto.CounsellingBy;
        patient.DelayReason = dto.DelayReason;
        patient.SurveyReturned = dto.SurveyReturned;
        patient.IsEnglishFirstLanguage = dto.IsEnglishFirstLanguage;
        patient.TreatmentId = dto.TreatmentId;
        patient.Adjuvant = dto.Adjuvant;
        patient.Palliative = dto.Palliative;
        patient.DispensedDate = dto.DispensedDate;
        patient.ImagingDate = dto.ImagingDate;
        patient.ResultsDate = dto.ResultsDate;
        patient.NextCycleDue = dto.NextCycleDue;
        patient.NextAppointment = dto.NextAppointment;
        patient.Comments = dto.Comments;
    }

    private string GetCurrentUser()
    {
        // For now, return a default user. In real implementation, this would come from JWT token
        return User?.Identity?.Name ?? "system";
    }


    private async Task<IEnumerable<PatientTrackingDto>> GetPatientsForExport(string? filter, string? searchTerm)
    {
        IEnumerable<PatientTracking> patients;

        if (!string.IsNullOrEmpty(searchTerm))
        {
            patients = await _unitOfWork.Patients.SearchAsync(searchTerm);
        }
        else if (!string.IsNullOrEmpty(filter))
        {
            patients = filter.ToLower() switch
            {
                "awaiting-counselling" => await _unitOfWork.Patients.GetAwaitingCounsellingAsync(),
                "awaiting-treatment" => await _unitOfWork.Patients.GetAwaitingTreatmentAsync(),
                "follow-up-due" => await _unitOfWork.Patients.GetFollowUpDueAsync(),
                _ => await _unitOfWork.Patients.GetAllAsync()
            };
        }
        else
        {
            patients = await _unitOfWork.Patients.GetAllAsync();
        }

        return patients.Select(MapToDto);
 }

       

    private StatisticsDto CalculateStatistics(List<PatientTracking> patients)
    {
        var statistics = new StatisticsDto();

        // Summary Metrics
        statistics.SummaryMetrics = CalculateSummaryMetrics(patients);

        // Time Distributions
        statistics.WaitTimeDistribution = CalculateWaitTimeDistribution(patients);
        statistics.TreatmentTimeDistribution = CalculateTreatmentTimeDistribution(patients);

        // Treatment Types
        statistics.TreatmentTypes = CalculateTreatmentTypes(patients);

        // Counsellor Metrics (simplified - using CreatedBy as proxy for counsellor)
        statistics.CounsellorMetrics = CalculateCounsellorMetrics(patients);

        // Demographics
        statistics.Demographics = CalculateDemographics(patients);

        // Operational Metrics
        statistics.OperationalMetrics = CalculateOperationalMetrics(patients);

        // Referral Trends
        statistics.ReferralTrends = CalculateReferralTrends(patients);

        return statistics;
    }

    private SummaryMetricsDto CalculateSummaryMetrics(List<PatientTracking> patients)
    {
        var waitTimes = patients.Where(p => p.WaitTimeReferralToCounselling.HasValue)
                               .Select(p => p.WaitTimeReferralToCounselling!.Value)
                               .ToList();

        var treatmentTimes = patients.Where(p => p.TreatTime.HasValue)
                                   .Select(p => p.TreatTime!.Value)
                                   .ToList();

        var targetDays = 5;
        var withinTarget = waitTimes.Count(wt => wt <= targetDays);

        return new SummaryMetricsDto
        {
            TotalPatients = patients.Count,
            AverageWaitTime = waitTimes.Any() ? waitTimes.Average() : null,
            MedianWaitTime = waitTimes.Any() ? CalculateMedian(waitTimes) : null,
            MaxWaitTime = waitTimes.Any() ? waitTimes.Max() : null,
            MinWaitTime = waitTimes.Any() ? waitTimes.Min() : null,
            AverageTreatmentTime = treatmentTimes.Any() ? treatmentTimes.Average() : null,
            MedianTreatmentTime = treatmentTimes.Any() ? CalculateMedian(treatmentTimes) : null,
            MaxTreatmentTime = treatmentTimes.Any() ? treatmentTimes.Max() : null,
            MinTreatmentTime = treatmentTimes.Any() ? treatmentTimes.Min() : null,
            SurveyCompletionRate = patients.Count > 0 ? (double)patients.Count(p => p.SurveyReturned) / patients.Count * 100 : 0,
            PatientsSeenWithinTargetTime = waitTimes.Count > 0 ? (double)withinTarget / waitTimes.Count * 100 : 0,
            TargetDays = targetDays
        };
    }

    private double CalculateMedian(List<int> values)
    {
        var sorted = values.OrderBy(x => x).ToList();
        var count = sorted.Count;
        if (count % 2 == 0)
        {
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }
        return sorted[count / 2];
    }

    private List<TimeDistributionDto> CalculateWaitTimeDistribution(List<PatientTracking> patients)
    {
        var waitTimes = patients.Where(p => p.WaitTimeReferralToCounselling.HasValue)
                               .Select(p => p.WaitTimeReferralToCounselling!.Value)
                               .ToList();

        return CalculateTimeDistribution(waitTimes);
    }

    private List<TimeDistributionDto> CalculateTreatmentTimeDistribution(List<PatientTracking> patients)
    {
        var treatmentTimes = patients.Where(p => p.TreatTime.HasValue)
                                   .Select(p => p.TreatTime!.Value)
                                   .ToList();

        return CalculateTimeDistribution(treatmentTimes);
    }

    private List<TimeDistributionDto> CalculateTimeDistribution(List<int> times)
    {
        var ranges = new List<TimeDistributionDto>
        {
            new() { Range = "0-5 days", MinDays = 0, MaxDays = 5, Count = 0 },
            new() { Range = "6-10 days", MinDays = 6, MaxDays = 10, Count = 0 },
            new() { Range = "11-15 days", MinDays = 11, MaxDays = 15, Count = 0 },
            new() { Range = "16-20 days", MinDays = 16, MaxDays = 20, Count = 0 },
            new() { Range = "21-30 days", MinDays = 21, MaxDays = 30, Count = 0 },
            new() { Range = "31+ days", MinDays = 31, MaxDays = int.MaxValue, Count = 0 }
        };

        foreach (var time in times)
        {
            var range = ranges.FirstOrDefault(r => time >= r.MinDays && time <= r.MaxDays);
            if (range != null)
            {
                range.Count++;
            }
        }

        return ranges;
    }

    private List<TreatmentTypeDto> CalculateTreatmentTypes(List<PatientTracking> patients)
    {
        var totalCount = patients.Count;
        if (totalCount == 0)
        {
            return new List<TreatmentTypeDto>(); // nothing to report
        }

        var result = new List<TreatmentTypeDto>();

        // Count patients without a TreatmentId (null/unspecified)
        var patientsWithoutTreatment = patients.Where(p => !p.TreatmentId.HasValue).ToList();
        if (patientsWithoutTreatment.Count > 0)
        {
            result.Add(new TreatmentTypeDto
            {
                TreatmentName = "Unspecified",
                PatientCount = patientsWithoutTreatment.Count,
                Percentage = (double)patientsWithoutTreatment.Count / totalCount * 100d
            });
        }

        // Group patients with TreatmentId by TreatmentId
        var patientsWithTreatment = patients
            .Where(p => p.TreatmentId.HasValue)
            .ToList();

        var grouped = patientsWithTreatment
            .GroupBy(p => p.TreatmentId!.Value)
            .Select(g =>
            {
                // Prefer navigation property name if loaded, else fallback
                var first = g.First();
                var name = first.TreatmentLookup?.Name;

                // If navigation not loaded (null), attempt to fall back to the TreatmentName
                // already exposed via DTO mapping; if still null use a placeholder.
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = "Unknown";
                }

                var count = g.Count();

                return new TreatmentTypeDto
                {
                    TreatmentName = name,
                    PatientCount = count,
                    Percentage = (double)count / totalCount * 100d
                };
            })
            .ToList();

        result.AddRange(grouped);

        return result
            .OrderByDescending(x => x.PatientCount)
            .ThenBy(x => x.TreatmentName)
            .ToList();
    }

    private List<CounsellorMetricDto> CalculateCounsellorMetrics(List<PatientTracking> patients)
    {
        // Get all patients with counselling date
        var counselledPatients = patients.Where(p => p.CounsellingDate.HasValue).ToList();
        
        var result = new List<CounsellorMetricDto>();
        
        // Handle patients with null/empty CreatedBy as "Unspecified"
        var unspecifiedGroup = counselledPatients.Where(p => string.IsNullOrEmpty(p.CreatedBy)).ToList();
        if (unspecifiedGroup.Any())
        {
            var waitTimes = unspecifiedGroup
                .Where(p => p.WaitTimeReferralToCounselling.HasValue)
                .Select(p => p.WaitTimeReferralToCounselling!.Value)
                .ToList();

            result.Add(new CounsellorMetricDto
            {
                CounsellorName = "Unspecified",
                PatientCount = unspecifiedGroup.Count,
                AverageWaitTime = waitTimes.Any() ? waitTimes.Average() : (double?)null
            });
        }

        // Group patients with non-empty CreatedBy
        var counsellorGroups = counselledPatients
            .Where(p => !string.IsNullOrEmpty(p.CreatedBy))
            .GroupBy(p => p.CreatedBy!)
            .Select(g => new CounsellorMetricDto
            {
                CounsellorName = g.Key,
                PatientCount = g.Count(),
                AverageWaitTime = g.Where(p => p.WaitTimeReferralToCounselling.HasValue)
                                 .Select(p => p.WaitTimeReferralToCounselling!.Value)
                                 .Any()
                                 ? g.Where(p => p.WaitTimeReferralToCounselling.HasValue)
                                   .Select(p => p.WaitTimeReferralToCounselling!.Value)
                                   .Average()
                                 : (double?)null
            })
            .ToList();

        result.AddRange(counsellorGroups);

        return result
            .OrderByDescending(c => c.PatientCount)
            .ToList();
    }

    private DemographicsDto CalculateDemographics(List<PatientTracking> patients)
    {
        var englishCount = patients.Count(p => p.IsEnglishFirstLanguage);
        var otherCount = patients.Count - englishCount;
        var surveyReturnedCount = patients.Count(p => p.SurveyReturned);
        var surveyNotReturnedCount = patients.Count - surveyReturnedCount;

        return new DemographicsDto
        {
            EnglishFirstLanguageCount = englishCount,
            OtherLanguageCount = otherCount,
            EnglishFirstLanguagePercentage = patients.Count > 0 ? (double)englishCount / patients.Count * 100 : 0,
            OtherLanguagePercentage = patients.Count > 0 ? (double)otherCount / patients.Count * 100 : 0,
            SurveyReturnedCount = surveyReturnedCount,
            SurveyNotReturnedCount = surveyNotReturnedCount,
            SurveyReturnedPercentage = patients.Count > 0 ? (double)surveyReturnedCount / patients.Count * 100 : 0
        };
    }

    private OperationalMetricsDto CalculateOperationalMetrics(List<PatientTracking> patients)
    {
        var awaitingCounselling = patients.Count(p => !p.CounsellingDate.HasValue);
        var awaitingTreatment = patients.Count(p => p.CounsellingDate.HasValue && !p.DispensedDate.HasValue);

        var nextAppointmentDistribution = CalculateNextAppointmentDistribution(patients);

        return new OperationalMetricsDto
        {
            AwaitingCounsellingCount = awaitingCounselling,
            AwaitingTreatmentCount = awaitingTreatment,
            NextAppointmentDistribution = nextAppointmentDistribution
        };
    }

    private List<NextAppointmentDistributionDto> CalculateNextAppointmentDistribution(List<PatientTracking> patients)
    {
        var today = DateTime.Today;
        var patientsWithNextAppt = patients.Where(p => p.NextAppointment.HasValue).ToList();

        var ranges = new List<NextAppointmentDistributionDto>
        {
            new() { DaysRange = "Overdue", Count = 0 },
            new() { DaysRange = "Today", Count = 0 },
            new() { DaysRange = "1-7 days", Count = 0 },
            new() { DaysRange = "8-14 days", Count = 0 },
            new() { DaysRange = "15-30 days", Count = 0 },
            new() { DaysRange = "31+ days", Count = 0 }
        };

        foreach (var patient in patientsWithNextAppt)
        {
            var daysFromToday = (patient.NextAppointment!.Value.Date - today).Days;

            if (daysFromToday < 0)
                ranges[0].Count++; // Overdue
            else if (daysFromToday == 0)
                ranges[1].Count++; // Today
            else if (daysFromToday <= 7)
                ranges[2].Count++; // 1-7 days
            else if (daysFromToday <= 14)
                ranges[3].Count++; // 8-14 days
            else if (daysFromToday <= 30)
                ranges[4].Count++; // 15-30 days
            else
                ranges[5].Count++; // 31+ days
        }

        return ranges;
    }

    private List<TrendDataDto> CalculateReferralTrends(List<PatientTracking> patients)
    {
        return patients
            .Where(p => p.ReferralDate.HasValue)
            .GroupBy(p => new { Year = p.ReferralDate!.Value.Year, Month = p.ReferralDate!.Value.Month })
            .Select(g => new TrendDataDto
            {
                Period = $"{g.Key.Year:0000}-{g.Key.Month:00}",
                Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                Count = g.Count()
            })
            .OrderBy(t => t.Date)
            .ToList();
    }
}