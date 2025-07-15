using Microsoft.AspNetCore.Mvc;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using SwimmingAcademy.Repositories;

namespace SwimmingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly IClubRepository _repository;

        public ClubController(IClubRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertNewClub([FromBody] InsertClubRequest request)
        {
            try
            {
                var success = await _repository.InsertNewClubAsync(request);

                if (success)
                    return Ok(new { message = "Club inserted successfully." });

                return StatusCode(500, new { message = "Failed to insert club." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while inserting club.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetClubs(int userId)
        {
            try
            {
                var clubs = await _repository.GetAllClubsAsync(userId);

                if (clubs == null || !clubs.Any())
                    return NotFound(new { message = "No clubs found." });

                return Ok(clubs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch clubs.",
                    error = ex.Message
                });
            }
        }
    }
}
