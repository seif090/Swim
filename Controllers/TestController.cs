using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwimmingAcademy.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SwimmingAcademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly SwimmingAcademyContext _context;
        private readonly IConfiguration _configuration;

        public TestController(SwimmingAcademyContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { 
                Status = "Healthy", 
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        [HttpGet("database")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    // Try to get a simple count
                    var userCount = await _context.users.CountAsync();
                    
                    return Ok(new { 
                        Status = "Database Connected", 
                        UserCount = userCount,
                        ConnectionString = _context.Database.GetConnectionString()?.Substring(0, 50) + "..."
                    });
                }
                else
                {
                    return StatusCode(500, new { 
                        Status = "Database Connection Failed",
                        Error = "Cannot connect to database"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Status = "Database Error",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace?.Substring(0, Math.Min(ex.StackTrace?.Length ?? 0, 500))
                });
            }
        }

        [HttpGet("sql-test")]
        public async Task<IActionResult> TestSqlConnection()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return StatusCode(500, new { 
                        Status = "Configuration Error",
                        Error = "Connection string is null or empty"
                    });
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    using (var command = new SqlCommand("SELECT COUNT(*) FROM users", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        
                        return Ok(new { 
                            Status = "SQL Connection Successful",
                            UserCount = result,
                            ServerVersion = connection.ServerVersion,
                            Database = connection.Database,
                            DataSource = connection.DataSource
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Status = "SQL Connection Error",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace?.Substring(0, Math.Min(ex.StackTrace?.Length ?? 0, 500))
                });
            }
        }

        [HttpGet("connection-string")]
        public IActionResult TestConnectionString()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
                
                return Ok(new { 
                    HasConnectionString = !string.IsNullOrEmpty(connectionString),
                    ConnectionStringLength = connectionString?.Length ?? 0,
                    ConnectionStringPreview = connectionString?.Substring(0, Math.Min(connectionString?.Length ?? 0, 100)) + "...",
                    Environment = environment,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Status = "Configuration Error",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace?.Substring(0, Math.Min(ex.StackTrace?.Length ?? 0, 500))
                });
            }
        }

        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return Ok(new { 
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                DetailedErrors = Environment.GetEnvironmentVariable("ASPNETCORE_DETAILEDERRORS"),
                CurrentDirectory = Environment.CurrentDirectory,
                MachineName = Environment.MachineName
            });
        }
    }
} 