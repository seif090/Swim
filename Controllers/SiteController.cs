using Microsoft.AspNetCore.Mvc;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;

namespace SwimmingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteController : ControllerBase
    {
        private readonly ISiteRepository _siteRepository;

        public SiteController(ISiteRepository siteRepository)
        {
            _siteRepository = siteRepository;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertNewSite([FromBody] InsertSiteRequest request)
        {
            try
            {
                var success = await _siteRepository.InsertNewSiteAsync(request);

                if (success)
                    return Ok(new { message = "Site inserted successfully." });

                return StatusCode(500, new { message = "Failed to insert site." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while inserting site.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSites(int userId)
        {
            try
            {
                var sites = await _siteRepository.GetAllSitesAsync(userId);

                if (sites == null || !sites.Any())
                    return NotFound(new { message = "No sites found." });

                return Ok(sites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch sites.",
                    error = ex.Message
                });
            }
        }
    }
}
