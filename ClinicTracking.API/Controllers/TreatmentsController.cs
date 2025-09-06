using ClinicTracking.API.DTOs;
using ClinicTracking.Core.Entities;
using ClinicTracking.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicTracking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TreatmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TreatmentsController> _logger;

    public TreatmentsController(IUnitOfWork unitOfWork, ILogger<TreatmentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TreatmentDto>>> GetAll()
    {
        try
        {
            var treatments = await _unitOfWork.Treatments.GetAllAsync();
            var treatmentDtos = new List<TreatmentDto>();

            foreach (var treatment in treatments)
            {
                var isInUse = await _unitOfWork.Treatments.IsInUseAsync(treatment.Id);
                treatmentDtos.Add(MapToDto(treatment, isInUse));
            }

            return Ok(treatmentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all treatments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TreatmentDto>> GetById(Guid id)
    {
        try
        {
            var treatment = await _unitOfWork.Treatments.GetByIdAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }

            var isInUse = await _unitOfWork.Treatments.IsInUseAsync(id);
            return Ok(MapToDto(treatment, isInUse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving treatment by id: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    //[Authorize(Roles = "ClinicAdmin")]
    public async Task<ActionResult<TreatmentDto>> Create([FromBody] CreateTreatmentDto createDto)
    {
        try
        {
            // Check if treatment with same name already exists
            var existingTreatment = await _unitOfWork.Treatments.GetByNameAsync(createDto.Name);
            if (existingTreatment != null)
            {
                return Conflict($"Treatment with name '{createDto.Name}' already exists");
            }

            var treatment = MapFromCreateDto(createDto);
            treatment.CreatedBy = User.Identity?.Name ?? "Unknown";

            await _unitOfWork.Treatments.AddAsync(treatment);
            await _unitOfWork.SaveChangesAsync();

            var isInUse = await _unitOfWork.Treatments.IsInUseAsync(treatment.Id);
            return CreatedAtAction(nameof(GetById), new { id = treatment.Id }, MapToDto(treatment, isInUse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating treatment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:guid}")]
    //[Authorize(Roles = "ClinicAdmin")]
    public async Task<ActionResult<TreatmentDto>> Update(Guid id, [FromBody] UpdateTreatmentDto updateDto)
    {
        try
        {
            var treatment = await _unitOfWork.Treatments.GetByIdAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }

            // Check if another treatment with same name already exists
            var existingTreatment = await _unitOfWork.Treatments.GetByNameAsync(updateDto.Name);
            if (existingTreatment != null && existingTreatment.Id != id)
            {
                return Conflict($"Treatment with name '{updateDto.Name}' already exists");
            }

            treatment.Name = updateDto.Name;
            treatment.Description = updateDto.Description;
            treatment.ModifiedBy = User.Identity?.Name ?? "Unknown";
            treatment.ModifiedOn = DateTime.UtcNow;

            await _unitOfWork.Treatments.UpdateAsync(treatment);
            await _unitOfWork.SaveChangesAsync();

            var isInUse = await _unitOfWork.Treatments.IsInUseAsync(id);
            return Ok(MapToDto(treatment, isInUse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating treatment with id: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:guid}")]
    //[Authorize(Roles = "ClinicAdmin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var treatment = await _unitOfWork.Treatments.GetByIdAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }

            // Check if treatment is in use
            var isInUse = await _unitOfWork.Treatments.IsInUseAsync(id);
            if (isInUse)
            {
                return BadRequest("Cannot delete treatment that is currently assigned to patients");
            }

            await _unitOfWork.Treatments.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting treatment with id: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private TreatmentDto MapToDto(Treatment treatment, bool isInUse)
    {
        return new TreatmentDto
        {
            Id = treatment.Id,
            Name = treatment.Name,
            Description = treatment.Description,
            IsAutoAdded = treatment.IsAutoAdded,
            CreatedBy = treatment.CreatedBy,
            CreatedOn = treatment.CreatedOn,
            ModifiedBy = treatment.ModifiedBy,
            ModifiedOn = treatment.ModifiedOn,
            IsInUse = isInUse
        };
    }

    private Treatment MapFromCreateDto(CreateTreatmentDto dto)
    {
        return new Treatment
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CreatedOn = DateTime.UtcNow
        };
    }
}