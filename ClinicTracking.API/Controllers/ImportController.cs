using ClinicTracking.API.DTOs;
using ClinicTracking.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicTracking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IImportService _importService;
    private readonly ILogger<ImportController> _logger;

    public ImportController(IImportService importService, ILogger<ImportController> logger)
    {
        _importService = importService;
        _logger = logger;
    }

    [HttpPost("excel")]
    public async Task<ActionResult<ImportResultDto>> ImportExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .xlsx files are supported");
            }

            // Get current user
            var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? 
                             User.FindFirst(ClaimTypes.Email)?.Value ?? 
                             "Unknown User";

            using var stream = file.OpenReadStream();
            
            // Validate file first
            var isValid = await _importService.ValidateExcelFileAsync(stream);
            if (!isValid)
            {
                return BadRequest("Invalid Excel file format. Please ensure the file has 'Name' and 'MRN' columns.");
            }

            // Reset stream position for import
            stream.Position = 0;
            
            var result = await _importService.ImportFromExcelAsync(stream, file.FileName, currentUser);
            
            if (result.Errors.Any())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Excel import");
            return StatusCode(500, "An error occurred during import");
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<bool>> ValidateExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .xlsx files are supported");
            }

            using var stream = file.OpenReadStream();
            var isValid = await _importService.ValidateExcelFileAsync(stream);
            
            return Ok(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Excel file");
            return StatusCode(500, "An error occurred during validation");
        }
    }
}