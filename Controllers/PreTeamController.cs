﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using SwimmingAcademy.Repositories;
using System.Threading.Tasks;

namespace SwimmingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreTeamController : ControllerBase
    {
        private readonly IPreTeamRepository _repo;
        private readonly ILogger<PreTeamController> _logger;

        public PreTeamController(IPreTeamRepository repo, ILogger<PreTeamController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreatePTeamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _repo.CreatePreTeam(request);
            return Ok(result);
        }

        [HttpPost("end")]
        public async Task<IActionResult> EndPreTeam([FromBody] EndPreTeamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _repo.EndPTeamAsync(request);
                return result ? Ok("PreTeam ended successfully.") : BadRequest("Failed to end PreTeam.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending PreTeam");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpPost("search-actions")]
        public async Task<IActionResult> SearchActions([FromBody] PreTeamActionSearchRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var actions = await _repo.SearchActionsAsync(request);
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching PreTeam actions");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpPost("search")]
        public async Task<IActionResult> SearchPTeam([FromBody] PTeamSearchRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _repo.SearchPTeamAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching PreTeam");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpGet("{pTeamId}/swimmers")]
        public async Task<IActionResult> GetSwimmerPTeamDetails(long pTeamId)
        {
            try
            {
                var result = await _repo.GetSwimmerPTeamDetailsAsync(pTeamId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching swimmer PreTeam details");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePTeam([FromBody] UpdatePTeamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _repo.UpdatePTeamAsync(request);
                return result ? Ok("PreTeam updated successfully.") : BadRequest("Failed to update PreTeam.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PreTeam");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpGet("{pTeamId}/details-tab")]
        public async Task<IActionResult> GetPTeamDetailsTab(long pTeamId)
        {
            try
            {
                var result = await _repo.GetPTeamDetailsTabAsync(pTeamId);
                return result is null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PTeam details tab");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpPost("save-pteam-transaction")]
        public async Task<IActionResult> SavePTeamTransaction([FromBody] SavePteamTransRequest request)
        {
            try
            {
                var response = await _repo.SavePTeamTransactionAsync(request);
                if (response == null)
                    return StatusCode(500, new { message = "Invoice Item not found or transaction failed." });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to process preteam transaction.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("view-possible-pteam/{swimmerId}")]
        public async Task<IActionResult> ViewPossiblePteam(long swimmerId)
        {
            try
            {
                var data = await _repo.ViewPossiblePteamAsync(swimmerId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch possible preteam options.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("summary/{site}")]
        public async Task<IActionResult> GetSummary(short site)
        {
            try
            {
                var result = await _repo.GetPTeamSummariesBySiteAsync(site);

                if (result == null || !result.Any())
                    return NotFound(new { message = "No PreTeam records found for this site." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load PreTeam summary.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

    }
}