using ClinicTracking.API.DTOs;
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

    public PatientsController(IUnitOfWork unitOfWork, ILogger<PatientsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
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
            DelayReason = patient.DelayReason,
            SurveyReturned = patient.SurveyReturned,
            IsEnglishFirstLanguage = patient.IsEnglishFirstLanguage,
            Treatment = patient.Treatment,
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
            DelayReason = dto.DelayReason,
            SurveyReturned = dto.SurveyReturned,
            IsEnglishFirstLanguage = dto.IsEnglishFirstLanguage,
            Treatment = dto.Treatment,
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
        patient.DelayReason = dto.DelayReason;
        patient.SurveyReturned = dto.SurveyReturned;
        patient.IsEnglishFirstLanguage = dto.IsEnglishFirstLanguage;
        patient.Treatment = dto.Treatment;
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
}