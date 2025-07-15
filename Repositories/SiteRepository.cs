using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SwimmingAcademy.Data;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using System.Data;

namespace SwimmingAcademy.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        private readonly SwimmingAcademyContext _context;
        private readonly ILogger<SiteRepository> _logger;

        public SiteRepository(SwimmingAcademyContext context, ILogger<SiteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> InsertNewSiteAsync(InsertSiteRequest request)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[dbo].[Insert_New_Site]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@description", request.Description));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting new site.");
                throw new Exception("An error occurred while inserting the site.");
            }
        }
        public async Task<List<SiteDto>> GetAllSitesAsync(int userId)
        {
            var sites = new List<SiteDto>();

            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[dbo].[View_Sites]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@userid", userId));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var site = new SiteDto
                    {
                        Description = reader["description"]?.ToString() ?? string.Empty,
                        SubId = Convert.ToInt32(reader["sub_id"])
                    };

                    sites.Add(site);
                }

                return sites;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching sites.");
                throw new Exception("Failed to retrieve sites.");
            }
        }
    }
}
