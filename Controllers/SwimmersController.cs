﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using SwimmingAcademy.Repositories;

namespace SwimmingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwimmersController : ControllerBase
    {
        private readonly ISwimmerRepository _repo;
        private readonly ILogger<SwimmersController> _logger;

        public SwimmersController(ISwimmerRepository repo, ILogger<SwimmersController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddSwimmer([FromBody] AddSwimmerRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var swimmerId = await _repo.AddSwimmerAsync(request);
                return swimmerId > 0
                    ? Ok(new { SwimmerID = swimmerId })
                    : BadRequest("Failed to add swimmer.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding swimmer");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("{swimmerID}/info-tab")]
        public async Task<IActionResult> GetSwimmerInfoTab(long swimmerID)
        {
            try
            {
                var result = await _repo.GetSwimmerInfoTabAsync(swimmerID);
                return result is null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSwimmerInfoTab failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("{swimmerID}/logs")]
        public async Task<IActionResult> GetSwimmerLogs(long swimmerID)
        {
            try
            {
                var result = await _repo.GetSwimmerLogsAsync(swimmerID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSwimmerLogs failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPut("change-site")]
        public async Task<IActionResult> ChangeSite([FromBody] ChangeSwimmerSiteRequest request)
        {
            try
            {
                var result = await _repo.ChangeSwimmerSiteAsync(request);
                return result > 0 ? Ok(new { SwimmerID = result }) : BadRequest("Site change failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangeSite failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpDelete("{swimmerID}")]
        public async Task<IActionResult> DeleteSwimmer(long swimmerID)
        {
            try
            {
                var result = await _repo.DeleteSwimmerAsync(swimmerID);
                return result ? Ok("Swimmer deleted successfully.") : BadRequest("Failed to delete swimmer.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteSwimmer failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        

       

        [HttpPost("search-actions")]
        public async Task<IActionResult> SearchSwimmerActions([FromBody] SwimmerActionSearchRequest request)
        {
            try
            {
                var actions = await _repo.SearchSwimmerActionsAsync(request);
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchSwimmerActions failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchSwimmers([FromBody] SwimmerSearchRequest request)
        {
            try
            {
                int filtersUsed = 0;
                if (request.SwimmerID.HasValue) filtersUsed++;
                if (!string.IsNullOrWhiteSpace(request.FullName)) filtersUsed++;
                if (!string.IsNullOrWhiteSpace(request.Year)) filtersUsed++;
                if (request.Level.HasValue && request.Level > 0) filtersUsed++;

                if (filtersUsed != 1)
                    return BadRequest("Please provide exactly one filter: SwimmerID, FullName, Year, or Level.");

                var result = await _repo.SearchSwimmersAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchSwimmers failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("{swimmerID}/technical-tab")]
        public async Task<IActionResult> GetSwimmerTechnicalTab(long swimmerID)
        {
            try
            {
                var result = await _repo.GetTechnicalTapAsync(swimmerID);
                return result is null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSwimmerTechnicalTab failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSwimmer([FromBody] UpdateSwimmerRequest request)
        {
            try
            {
                var updated = await _repo.UpdateSwimmerAsync(request);
                return updated ? Ok("Swimmer updated successfully.") : BadRequest("Update failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateSwimmer failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPut("update-level")]
        public async Task<IActionResult> UpdateSwimmerLevel([FromBody] UpdateSwimmerLevelRequest request)
        {
            try
            {
                var success = await _repo.UpdateSwimmerLevelAsync(request);
                return success ? Ok("Swimmer level updated successfully.") : BadRequest("Failed to update level.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateSwimmerLevel failed");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        
    }
}