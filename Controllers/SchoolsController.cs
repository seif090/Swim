using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using SwimmingAcademy.Repositories;

namespace SwimmingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolsController : ControllerBase
    {
        private readonly ISchoolRepository _repo;
        private readonly ILogger<SchoolsController> _logger;

        public SchoolsController(ISchoolRepository repo, ILogger<SchoolsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("create")]
        public IActionResult CreateSchool([FromBody] CreateSchoolRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _repo.CreateSchool(request);
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchSchools([FromBody] SchoolSearchRequest request)
        {
            int filtersUsed = 0;
            if (request.SchoolID != null) filtersUsed++;
            if (!string.IsNullOrWhiteSpace(request.FullName)) filtersUsed++;
            if (request.Level != null) filtersUsed++;
            if (request.Type != null) filtersUsed++;

            if (filtersUsed != 1)
                return BadRequest("Please provide exactly one filter: SchoolID, FullName, Level, or Type.");

            try
            {
                var result = await _repo.SearchSchoolsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchSchools");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSchool([FromBody] UpdateSchoolRequest request)
        {
            try
            {
                var updated = await _repo.UpdateSchoolAsync(request);
                return updated ? Ok("School updated successfully.") : BadRequest("Update failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateSchool");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost("end")]
        public async Task<IActionResult> EndSchool([FromBody] EndSchoolRequest request)
        {
            try
            {
                var result = await _repo.EndSchoolAsync(request);
                return result ? Ok("School ended successfully.") : BadRequest("Failed to end school.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EndSchool");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("{schoolID}/details-tab")]
        public async Task<IActionResult> GetSchoolDetailsTab(long schoolID)
        {
            try
            {
                var result = await _repo.GetSchoolDetailsTabAsync(schoolID);
                return result == null ? NotFound("School not found.") : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSchoolDetailsTab");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("{schoolID}/swimmers")]
        public async Task<IActionResult> GetSchoolSwimmerDetails(long schoolID)
        {
            try
            {
                var result = await _repo.GetSchoolSwimmerDetailsAsync(schoolID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSchoolSwimmerDetails");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost("search-actions")]
        public async Task<IActionResult> SearchSchoolActions([FromBody] SchoolActionSearchRequest request)
        {
            try
            {
                var actions = await _repo.SearchSchoolActionsAsync(request);
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchSchoolActions");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("view-possible-school")]
        public async Task<IActionResult> ViewPossibleSchool([FromQuery] long swimmerId, [FromQuery] short type)
        {
            try
            {
                var result = await _repo.ViewPossibleSchoolAsync(swimmerId, type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving school options.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("save-school-transaction")]
        public async Task<IActionResult> SaveSchoolTransaction([FromBody] SaveSchoolTransRequest request)
        {
            try
            {
                var result = await _repo.SaveSchoolTransactionAsync(request);
                if (result == null)
                    return StatusCode(500, new { message = "Transaction failed or invoice item not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to process school transaction.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("summary/{site}")]
        public async Task<IActionResult> GetSchoolSummary(short site)
        {
            try
            {
                var data = await _repo.GetSchoolSummariesBySiteAsync(site);

                if (data == null || !data.Any())
                    return NotFound(new { message = "No schools found for the specified site." });

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving school summary.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}