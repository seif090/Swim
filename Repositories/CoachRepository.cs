using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwimmingAcademy.Data;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using SwimmingAcademy.Models;
using System.Data;

namespace SwimmingAcademy.Repositories
{
    public class CoachRepository : ICoachRepository
    {
        private readonly SwimmingAcademyContext _context;
        private readonly ILogger<CoachRepository> _logger;

        public CoachRepository(SwimmingAcademyContext context, ILogger<CoachRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<FreeCoachDto> GetFreeCoaches(FreeCoachFilterRequest request)
        {
            var result = new List<FreeCoachDto>();

            using (SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                conn.Open();

                string infoTable = (request.Type == 8) ? "Schools.info" : "PreTeam.info";

                string query = $@"
                SELECT C.FullName, C.CoachID, I.StartTime, @FirstDay AS FirstDay
                FROM Coaches C
                LEFT JOIN {infoTable} I ON C.CoachID = I.CoachID
                WHERE C.CoachType = @Type AND C.Site = @Site";

                var coaches = new List<(int CoachID, string Name, TimeSpan? StartTime, string Day)>();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Type", request.Type);
                    cmd.Parameters.AddWithValue("@Site", request.Site);
                    cmd.Parameters.AddWithValue("@FirstDay", request.FirstDay);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int coachId = reader.GetInt32(1);
                            string name = reader.GetString(0);
                            TimeSpan? start = reader.IsDBNull(2) ? null : reader.GetTimeSpan(2);

                            coaches.Add((coachId, name, start, request.FirstDay));

                        }
                    }
                }

                // Filter out busy coaches
                var availableCoaches = coaches
                 .Where(c => !(c.StartTime.HasValue && c.StartTime.Value == request.StartTime && c.Day == request.FirstDay))
                 .Select(c => new FreeCoachDto
                 {
                     CoachId = c.CoachID,
                     Name = c.Name
                 })
                 .ToList();

                result = availableCoaches;
            }

            return result;
        }
        public async Task<bool> InsertNewCoachAsync(InsertCoachRequest request)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[dbo].[insert_New_Coach]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(new[]
                {
                new SqlParameter("@userid", request.UserId),
                new SqlParameter("@site", request.Site),
                new SqlParameter("@fullname", request.FullName),
                new SqlParameter("@birthdate", request.BirthDate),
                new SqlParameter("@gender", request.Gender),
                new SqlParameter("@IsActive", request.IsActive),
                new SqlParameter("@Type", request.Type),
                new SqlParameter("@MobileNumber", request.MobileNumber)
            });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while inserting new coach.");
                throw new Exception("Failed to insert new coach.");
            }
        }

        public async Task<bool> UpdateCoachAsync(UpdateCoachRequest request)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[dbo].[UPDATE_Coaches]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(new[]
                {
                new SqlParameter("@userid", request.UserId),
                new SqlParameter("@Site", request.Site),
                new SqlParameter("@coachid", request.CoachId),
                new SqlParameter("@fullname", request.FullName),
                new SqlParameter("@type", request.Type),
                new SqlParameter("@IsActive", request.IsActive)
            });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating coach record.");
                throw new Exception("Failed to update coach record.");
            }
        }
        public async Task<List<CoachesDto>> GetCoachesBySiteAsync(short site)
        {
            var result = new List<CoachesDto>();

            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[dbo].[View_Coaches]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@site", site));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var coach = new CoachesDto // Corrected type to match the expected type
                    {
                        CoachID = Convert.ToInt32(reader["CoachID"]),
                        FullName = reader["FullName"]?.ToString() ?? "",
                        Age = Convert.ToInt32(reader["Age"]),
                        CoachType = reader["coachType"]?.ToString() ?? "",
                        MobileNumber = reader["mobileNumber"]?.ToString() ?? ""
                    };

                    result.Add(coach);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching coaches");
                throw new Exception("Failed to retrieve coach list.");
            }
        }
    }
}

