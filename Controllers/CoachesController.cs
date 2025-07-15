using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwimmingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoachesController : ControllerBase
    {
        private readonly ICoachRepository _coachRepository;
        private readonly ILogger<CoachesController> _logger;

        public CoachesController(ICoachRepository coachRepository, ILogger<CoachesController> logger)
        {
            _coachRepository = coachRepository;
            _logger = logger;
        }

        [HttpPost("free")]
        public IActionResult GetFreeCoaches([FromBody] FreeCoachFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var coaches = _coachRepository.GetFreeCoaches(request);
            return Ok(coaches);
        }
        [HttpPost("insert")]
        public async Task<IActionResult> InsertCoach([FromBody] InsertCoachRequest request)
        {
            try
            {
                var success = await _coachRepository.InsertNewCoachAsync(request);
                if (success)
                    return Ok(new { message = "Coach inserted successfully." });

                return StatusCode(500, new { message = "Failed to insert coach." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error occurred while inserting coach.",
                    error = ex.Message
                });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCoach([FromBody] UpdateCoachRequest request)
        {
            try
            {
                bool result = await _coachRepository.UpdateCoachAsync(request);
                if (result)
                    return Ok(new { message = "Coach updated successfully." });

                return NotFound(new { message = "Coach not found or not updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error occurred while updating coach.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("site/{site}")]
        public async Task<IActionResult> GetCoachesBySite(short site)
        {
            try
            {
                var coaches = await _coachRepository.GetCoachesBySiteAsync(site);

                if (coaches == null || !coaches.Any())
                    return NotFound(new { message = "No coaches found for the site." });

                return Ok(coaches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching coaches.",
                    error = ex.Message
                });
            }
        }
    }
}
