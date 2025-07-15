using Microsoft.AspNetCore.Mvc;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;

namespace SwimmingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("add-site")]
        public async Task<IActionResult> AddSiteUser([FromBody] AddSiteUserRequest request)
        {
            try
            {
                await _userRepository.AddUserSiteAsync(request.UserId, request.Site);
                return Ok(new { message = "Site added to user successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add site for user {UserId}", request.UserId);
                return StatusCode(500, new { message = "Error adding site to user." });
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> InsertUser([FromBody] InsertUserRequest request)
        {
            try
            {
                int newUserId = await _userRepository.InsertUserAsync(request);
                return Ok(new { message = "User created successfully", userId = newUserId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting user.");
                return StatusCode(500, new { message = "Internal server error while inserting user." });
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                bool isSuccess = await _userRepository.UpdateUserAsync(request);
                if (isSuccess)
                    return Ok(new { message = "User updated successfully." });

                return NotFound(new { message = "User not found or update failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating user.");
                return StatusCode(500, new { message = "Internal server error while updating user." });
            }
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetUserSummary([FromQuery] short site)
        {
            try
            {
                var users = await _userRepository.GetUserSummaryBySiteAsync(site);

                if (users == null || !users.Any())
                    return NotFound(new { message = "No users found for the specified site." });

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user summary.");
                return StatusCode(500, new { message = "Internal server error while retrieving user summary." });
            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserDetailsAsync(userId);

                if (user == null || string.IsNullOrEmpty(user.FullName))
                    return NotFound(new { message = "User not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user data.");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}
