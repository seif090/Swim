using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SwimmingAcademy.Data;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace SwimmingAcademy.Repositories
{
    public class PreTeamRepository : IPreTeamRepository
    {
        private readonly SwimmingAcademyContext _context;
        private readonly ILogger<PreTeamRepository> _logger;

        public PreTeamRepository(SwimmingAcademyContext context, ILogger<PreTeamRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public CreatePreTeamResponse CreatePreTeam(CreatePTeamRequest request)
        {
            using (var conn = new SqlConnection(_context.Database.GetConnectionString()))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Insert into PreTeam.Info
                        string insertInfoQuery = @"
                        INSERT INTO PreTeam.Info
                        (PTeamLevel, CoachID, FirstDay, SecondDay, ThirdDay, site, createdAtSite, createdBy, createdAtDate, StartTime, EndTime)
                        VALUES
                        (@PTeamLevel, @CoachID, @FirstDay, @SecondDay, @ThirdDay, @Site, @Site, @UserId, GETDATE(), @StartTime, @EndTime);
                        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                        long pteamId;
                        using (var cmd = new SqlCommand(insertInfoQuery, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@PTeamLevel", request.PreTeamLevel);
                            cmd.Parameters.AddWithValue("@CoachID", request.CoachID);
                            cmd.Parameters.AddWithValue("@FirstDay", request.FirstDay);
                            cmd.Parameters.AddWithValue("@SecondDay", request.SecondDay);
                            cmd.Parameters.AddWithValue("@ThirdDay", request.ThirdDay);
                            cmd.Parameters.AddWithValue("@Site", request.Site);
                            cmd.Parameters.AddWithValue("@UserId", request.UserId);
                            cmd.Parameters.AddWithValue("@StartTime", request.StartTime);
                            cmd.Parameters.AddWithValue("@EndTime", request.EndTime);

                            pteamId = (long)cmd.ExecuteScalar();
                        }

                        // Step 2: Insert into PreTeam.Log
                        string insertLogQuery = @"
                        INSERT INTO PreTeam.Log
                        (PteamID, ActionID, createdAtSite, CreatedBy, CreatedAtDate)
                        VALUES
                        (@PteamID, 21, @Site, @UserId, GETDATE());";

                        using (var cmd = new SqlCommand(insertLogQuery, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@PteamID", pteamId);
                            cmd.Parameters.AddWithValue("@Site", request.Site);
                            cmd.Parameters.AddWithValue("@UserId", request.UserId);

                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();

                        return new CreatePreTeamResponse { PteamID = pteamId };
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> EndPTeamAsync(EndPreTeamRequest request)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[PreTeam].[EndPreTeam]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@PteamID", request.PteamID));
                command.Parameters.Add(new SqlParameter("@userID", request.UserID));
                command.Parameters.Add(new SqlParameter("@site", request.Site));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending PreTeam for PteamID {PteamID}", request.PteamID);
                throw new Exception("An error occurred while ending the PreTeam.");
            }
        }
        public async Task<IEnumerable<ActionNameDto>> SearchActionsAsync(PreTeamActionSearchRequest request)
        {
            var actions = new List<ActionNameDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[PreTeam].[SerachActions]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@UserID", request.UserID));
                command.Parameters.Add(new SqlParameter("@PTeamID", request.PTeamID));
                command.Parameters.Add(new SqlParameter("@userSite", request.UserSite));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    actions.Add(new ActionNameDto
                    {
                        ActionName = reader.IsDBNull(0) ? "" : reader.GetString(0)
                    });
                }
                return actions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching PreTeam actions for UserID {UserID}, PTeamID {PTeamID}, UserSite {UserSite}", request.UserID, request.PTeamID, request.UserSite);
                throw new Exception("An error occurred while searching PreTeam actions.");
            }
        }
        public async Task<IEnumerable<PTeamSearchResultDto>> SearchPTeamAsync(PTeamSearchRequest request)
        {
            var result = new List<PTeamSearchResultDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[PreTeam].[ShowPTeam]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@PTeamID", (object?)request.PTeamID ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@FullName", (object?)request.FullName ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@level", (object?)request.Level ?? DBNull.Value));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new PTeamSearchResultDto
                    {
                        PTeamID = reader.IsDBNull(0) ? 0 : reader.GetInt64(0),
                        CoachName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Level = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Days = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        FromTo = reader.IsDBNull(4) ? "" : reader.GetString(4)
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching PreTeam with request {@Request}", request);
                throw new Exception("An error occurred while searching PreTeam.");
            }
        }
        public async Task<IEnumerable<SwimmerPTeamDetailsDto>> GetSwimmerPTeamDetailsAsync(long pTeamId)
        {
            var result = new List<SwimmerPTeamDetailsDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[PreTeam].[SwimmerDetails_Tab]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@pteamID", pTeamId));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new SwimmerPTeamDetailsDto
                    {
                        FullName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Attendance = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        SwimmerLevel = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        LastStar = reader.IsDBNull(3) ? "" : reader.GetString(3)
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching swimmer PreTeam details for PTeamID {PTeamID}", pTeamId);
                throw new Exception("An error occurred while fetching swimmer PreTeam details.");
            }
        }
        public async Task<bool> UpdatePTeamAsync(UpdatePTeamRequest request)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[PreTeam].[Updated]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@PTeamID", request.PTeamID));
                command.Parameters.Add(new SqlParameter("@coachID", request.CoachID));
                command.Parameters.Add(new SqlParameter("@FirstDay", request.FirstDay));
                command.Parameters.Add(new SqlParameter("@SecondDay", request.SecondDay));
                command.Parameters.Add(new SqlParameter("@ThirdDay", request.ThirdDay));
                command.Parameters.Add(new SqlParameter("@StartTime", request.StartTime));
                command.Parameters.Add(new SqlParameter("@EndTime", request.EndTime));
                command.Parameters.Add(new SqlParameter("@site", request.Site));
                command.Parameters.Add(new SqlParameter("@user", request.User));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PreTeam with ID {PTeamID}", request.PTeamID);
                throw new Exception("An error occurred while updating the PreTeam.");
            }
        }
        //public async Task<long> CreatePreTeamAsync(CreatePTeamRequest request)
        //{
        //    try
        //    {
        //        using var conn = _context.Database.GetDbConnection();
        //        using var command = conn.CreateCommand();

        //        command.CommandText = "[PreTeam].[Create_PTeam]";
        //        command.CommandType = CommandType.StoredProcedure;

        //        command.Parameters.Add(new SqlParameter("@PTeamLevel", request.PreTeamLevel));
        //        command.Parameters.Add(new SqlParameter("@CoachID", request.CoachID));
        //        command.Parameters.Add(new SqlParameter("@FirstDay", request.FirstDay ?? (object)DBNull.Value));
        //        command.Parameters.Add(new SqlParameter("@SecondDay", request.SecondDay ?? (object)DBNull.Value));
        //        command.Parameters.Add(new SqlParameter("@ThirdDay", request.ThirdDay ?? (object)DBNull.Value));
        //        command.Parameters.Add(new SqlParameter("@site", request.Site));
        //        command.Parameters.Add(new SqlParameter("@user", request.UserId);
        //        command.Parameters.Add(new SqlParameter("@startTime", request.StartTime));
        //        command.Parameters.Add(new SqlParameter("@EndTime", request.EndTime));

        //        if (conn.State != ConnectionState.Open)
        //            await conn.OpenAsync();

        //        var result = await command.ExecuteScalarAsync();
        //        return result is null ? 0 : Convert.ToInt64(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating PreTeam");
        //        throw new Exception("An error occurred while creating the PreTeam.");
        //    }
        //}
        public async Task<PTeamDetailsTabDto?> GetPTeamDetailsTabAsync(long pTeamId)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[PreTeam].[PTeamDetails_tab]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@PTeamID", pTeamId));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new PTeamDetailsTabDto
                    {
                        FullName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        FirstDay = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        SecondDay = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        ThirdDay = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        StartTime = reader.IsDBNull(4) ? "" : reader.GetValue(4)?.ToString() ?? "",
                        EndTime = reader.IsDBNull(5) ? "" : reader.GetValue(5)?.ToString() ?? "",
                        IsEnded = !reader.IsDBNull(6) && reader.GetBoolean(6)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PTeam details tab for PTeamID {PTeamID}", pTeamId);
                throw new Exception("An error occurred while fetching PTeam details tab.");
            }
        }
        public async Task<SavePTeamTransResponse?> SavePTeamTransactionAsync(SavePteamTransRequest request)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[SavePteam_Trans]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(new[]
                {
                new SqlParameter("@swimmerID", request.SwimmerID),
                new SqlParameter("@PTeamID", request.PTeamID),
                new SqlParameter("@DuarationsInMonths", request.DuarationsInMonths),
                new SqlParameter("@user", request.UserId),
                new SqlParameter("@site", request.Site)
            });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                SavePTeamTransResponse? result = null;

                using var reader = await command.ExecuteReaderAsync();

                // Read the invoice result (from @Inv temp table)
                if (await reader.ReadAsync())
                {
                    result = new SavePTeamTransResponse
                    {
                        SwimmerID = request.SwimmerID,
                        PTeamID = request.PTeamID,
                        InvItem = reader["InvItem"]?.ToString() ?? string.Empty,
                        Value = reader["Value"] is decimal val ? val : Convert.ToDecimal(reader["Value"])
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute SavePteam_Trans stored procedure.");
                throw new Exception("An error occurred while saving preteam transaction.");
            }
        }
        public async Task<ViewPossiblePteamResponse> ViewPossiblePteamAsync(long swimmerId)
        {
            var result = new ViewPossiblePteamResponse();

            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Swimmers].[ViewPossible_Pteam]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@swimmerID", SqlDbType.BigInt) { Value = swimmerId });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                // First result set: PTeam data
                while (await reader.ReadAsync())
                {
                    result.PTeams.Add(new PTeamDto
                    {
                        PTeamID = reader["PTeamID"] is long id ? id : Convert.ToInt64(reader["PTeamID"]),
                        CoachName = reader["CoachName"]?.ToString() ?? string.Empty,
                        Dayes = reader["Dayes"]?.ToString() ?? string.Empty,
                        FromTo = reader["FromTo"]?.ToString() ?? string.Empty
                    });
                }

                // Second result set: Invoice items
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.InvoiceItems.Add(new InvoiceItemDto
                        {
                            ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                            DurationInMonths = reader["DurationInMonths"] is short d ? d : Convert.ToInt16(reader["DurationInMonths"]),
                            Amount = reader["Amount"] is decimal a ? a : Convert.ToDecimal(reader["Amount"])
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching possible PTeam data for swimmer ID {SwimmerId}", swimmerId);
                throw new Exception("An error occurred while retrieving pre-team data.");
            }
        }
        public async Task<List<PTeamSummaryDto>> GetPTeamSummariesBySiteAsync(short site)
        {
            var result = new List<PTeamSummaryDto>();

            // Fix for CS1929: Replace the incorrect usage of _context.GetConnectionString with the correct way to retrieve the connection string from the DbContext.

            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            using var command = new SqlCommand("dbo.Show_PTeam_Summary", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@site", site);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var dto = new PTeamSummaryDto
                {
                    PTeamID = reader.GetInt32(reader.GetOrdinal("PTeamID")),
                    CoachFullName = reader.GetString(reader.GetOrdinal("FullName")),
                    PTeamLevel = reader.GetString(reader.GetOrdinal("PTeamLevel")),
                    Dayes = reader.GetString(reader.GetOrdinal("Dayes")),
                    FromTo = reader.GetString(reader.GetOrdinal("From / To"))
                };

                result.Add(dto);
            }

            return result;
        }

    }
}