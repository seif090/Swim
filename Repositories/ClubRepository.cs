using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SwimmingAcademy.Data;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using System.Data;

namespace SwimmingAcademy.Repositories
{
    public class ClubRepository : IClubRepository
    {
        private readonly SwimmingAcademyContext _context;
        private readonly ILogger<ClubRepository> _logger;

        public ClubRepository(SwimmingAcademyContext context, ILogger<ClubRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> InsertNewClubAsync(InsertClubRequest request)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection(); // Ensure the correct namespace is included
                using var command = conn.CreateCommand();

                command.CommandText = "[dbo].[Insert_New_Club]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@description", request.Description));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting new club");
                throw new Exception("An error occurred while inserting the club.");
            }
        }
        public async Task<List<ClubDto>> GetAllClubsAsync(int userId)
        {
            var clubs = new List<ClubDto>();

            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[dbo].[View_Clubs]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@userid", userId));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    clubs.Add(new ClubDto
                    {
                        Description = reader["description"]?.ToString() ?? string.Empty,
                        SubId = Convert.ToInt32(reader["sub_id"])
                    });
                }

                return clubs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch clubs.");
                throw new Exception("An error occurred while retrieving clubs.");
            }
        }
    }
    
    
}
