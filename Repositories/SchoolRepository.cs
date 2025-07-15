using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SwimmingAcademy.Data;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using System.Data;

namespace SwimmingAcademy.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly SwimmingAcademyContext _context;
        private readonly ILogger<SchoolRepository> _logger;

        public SchoolRepository(SwimmingAcademyContext context, ILogger<SchoolRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public CreateSchoolResponse CreateSchool(CreateSchoolRequest request)
        {
            using var conn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                // Step 1: Insert into Schools.info
                string insertSchoolQuery = @"
                INSERT INTO Schools.info
                (schoolLevel, CoachID, FirstDay, SecondDay, SchoolType, site, createdAtSite, createdBy, createdAtDate, startTime, Endtime)
                VALUES
                (@SchoolLevel, @CoachID, @FirstDay, @SecondDay, @Type, @Site, @Site, @UserId, GETDATE(), @StartTime, @EndTime);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                long schoolId;

                using (var cmd = new SqlCommand(insertSchoolQuery, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@SchoolLevel", request.SchoolLevel);
                    cmd.Parameters.AddWithValue("@CoachID", request.CoachID);
                    cmd.Parameters.AddWithValue("@FirstDay", request.FirstDay);
                    cmd.Parameters.AddWithValue("@SecondDay", request.SecondDay);
                    cmd.Parameters.AddWithValue("@Type", request.Type);
                    cmd.Parameters.AddWithValue("@Site", request.Site);
                    cmd.Parameters.AddWithValue("@UserId", request.UserId);
                    cmd.Parameters.AddWithValue("@StartTime", request.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", request.EndTime);

                    schoolId = (long)cmd.ExecuteScalar();
                }

                // Step 2: Insert into Schools.log
                string insertLogQuery = @"
                INSERT INTO Schools.log
                (schoolID, ActionID, createdAtSite, CreatedBy, CreatedAtDate)
                VALUES
                (@SchoolID, 12, @Site, @UserId, GETDATE());";

                using (var cmd = new SqlCommand(insertLogQuery, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@SchoolID", schoolId);
                    cmd.Parameters.AddWithValue("@Site", request.Site);
                    cmd.Parameters.AddWithValue("@UserId", request.UserId);

                    cmd.ExecuteNonQuery();
                }

                tran.Commit();

                return new CreateSchoolResponse { SchoolID = schoolId };
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }





        public async Task<bool> UpdateSchoolAsync(UpdateSchoolRequest req)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Schools].[Updated]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(new[]
                {
            new SqlParameter("@schoolID", req.SchoolID),
            new SqlParameter("@coachID", req.CoachID),
            new SqlParameter("@FirstDay", req.FirstDay ?? (object)DBNull.Value),
            new SqlParameter("@SecondDay", req.SecondDay ?? (object)DBNull.Value),
            new SqlParameter("@StartTime", req.StartTime),
            new SqlParameter("@EndTime", req.EndTime),
            new SqlParameter("@Type", req.Type),
            new SqlParameter("@site", req.Site),
            new SqlParameter("@user", req.User)
        });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateSchoolAsync for SchoolID {SchoolID}", req.SchoolID);
                throw new Exception("An error occurred while updating the school.");
            }
        }

        public async Task<bool> EndSchoolAsync(EndSchoolRequest req)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Schools].[EndSchool]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(new[]
                {
            new SqlParameter("@schoolID", req.SchoolID),
            new SqlParameter("@userID", req.UserID),
            new SqlParameter("@site", req.Site)
        });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EndSchoolAsync for SchoolID {SchoolID}", req.SchoolID);
                throw new Exception("An error occurred while ending the school.");
            }
        }

        public async Task<IEnumerable<SchoolSearchResultDto>> SearchSchoolsAsync(SchoolSearchRequest req)
        {
            var result = new List<SchoolSearchResultDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Schools].[ShowSchool]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@SchoolID", (object?)req.SchoolID ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@FullName", (object?)req.FullName ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@level", (object?)req.Level ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@type", (object?)req.Type ?? DBNull.Value));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new SchoolSearchResultDto
                    {
                        CoachName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Level = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Type = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Days = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        FromTo = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        NumberCapacity = reader.IsDBNull(5) ? "" : reader.GetString(5)
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchSchoolsAsync");
                throw new Exception("An error occurred while searching for schools.");
            }
        }
        public async Task<SchoolDetailsTabDto?> GetSchoolDetailsTabAsync(long schoolID)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Schools].[SchoolDetalis_Tab]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolID", schoolID));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new SchoolDetailsTabDto
                    {
                        FullName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        FirstDay = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        SecondDay = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        StartTime = reader.IsDBNull(3) ? "" : TimeSpan.TryParse(reader.GetValue(3)?.ToString(), out var st) ? st.ToString(@"hh\:mm") : "",
                        EndTime = reader.IsDBNull(4) ? "" : TimeSpan.TryParse(reader.GetValue(4)?.ToString(), out var et) ? et.ToString(@"hh\:mm") : "",
                        Capacity = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                        NumberOfSwimmers = reader.IsDBNull(6) ? "0" : reader.GetString(6),
                        IsEnded = !reader.IsDBNull(7) && reader.GetBoolean(7)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSchoolDetailsTabAsync for SchoolID {SchoolID}", schoolID);
                throw new Exception("An error occurred while retrieving school details.");
            }
        }

        public async Task<IEnumerable<SchoolSwimmerDetailsDto>> GetSchoolSwimmerDetailsAsync(long schoolID)
        {
            var result = new List<SchoolSwimmerDetailsDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Schools].[SwimmerDetails_Tab]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@schoolID", schoolID));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new SchoolSwimmerDetailsDto
                    {
                        FullName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Attendence = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        SwimmerLevel = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSchoolSwimmerDetailsAsync for SchoolID {SchoolID}", schoolID);
                throw new Exception("An error occurred while retrieving school swimmer details.");
            }
        }
        public async Task<IEnumerable<ActionNameDto>> SearchSchoolActionsAsync(SchoolActionSearchRequest req)
        {
            var result = new List<ActionNameDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Schools].[SearchActions]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@UserID", req.UserID));
                command.Parameters.Add(new SqlParameter("@SchoolID", req.SchoolID));
                command.Parameters.Add(new SqlParameter("@userSite", req.UserSite));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new ActionNameDto
                    {
                        ActionName = reader.IsDBNull(0) ? "" : reader.GetString(0)
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchSchoolActionsAsync for UserID {UserID}, SchoolID {SchoolID}, UserSite {UserSite}", req.UserID, req.SchoolID, req.UserSite);
                throw new Exception("An error occurred while searching for school actions.");
            }
        }

        public async Task<ViewPossibleSchoolResponse> ViewPossibleSchoolAsync(long swimmerId, short type)
        {
            var response = new ViewPossibleSchoolResponse();

            try
            {
                await using var conn = _context.Database.GetDbConnection();
                await using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[ViewPossible_School]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@swimmerID", SqlDbType.BigInt) { Value = swimmerId });
                command.Parameters.Add(new SqlParameter("@Type", SqlDbType.SmallInt) { Value = type });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await using var reader = await command.ExecuteReaderAsync();

                // First result set: Schools
                while (reader.Read())
                {
                    response.Schools.Add(new SchoolDto
                    {
                        SchoolID = reader["SchoolID"] is long id ? id : Convert.ToInt64(reader["SchoolID"]),
                        CoachName = reader["CoachName"]?.ToString() ?? "",
                        Days = reader["Dayes"]?.ToString() ?? "",
                        FromTo = reader["FromTo"]?.ToString() ?? ""
                    });
                }

                // Second result set: Invoices
                if (await reader.NextResultAsync())
                {
                    while (reader.Read())
                    {
                        response.InvoiceItems.Add(new InvoiceItemDto
                        {
                            ItemName = reader["ItemName"]?.ToString() ?? "",
                            DurationInMonths = reader["DurationInMonths"] is short d ? d : Convert.ToInt16(reader["DurationInMonths"]),
                            Amount = reader["Amount"] is decimal a ? a : Convert.ToDecimal(reader["Amount"])
                        });
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewPossibleSchoolAsync");
                throw new Exception("An error occurred while retrieving school options.");
            }
        }

        public async Task<SaveSchoolTransResponse?> SaveSchoolTransactionAsync(SaveSchoolTransRequest req)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[SaveSchool_Trans]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(new[]
                {
                new SqlParameter("@swimmerID", req.SwimmerID),
                new SqlParameter("@SchoolID", req.SchoolID),
                new SqlParameter("@DuarationsInMonths", req.DuarationsInMonths),
                new SqlParameter("@user", req.UserId),
                new SqlParameter("@site", req.Site)
            });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                SaveSchoolTransResponse? response = null;

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    response = new SaveSchoolTransResponse
                    {
                        SwimmerID = req.SwimmerID,
                        SchoolID = req.SchoolID,
                        InvItem = reader.IsDBNull(0) ? "" : reader.GetValue(0)?.ToString() ?? "",
                        Value = reader.IsDBNull(1) ? 0 : Convert.ToDecimal(reader.GetValue(1))
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveSchoolTransactionAsync");
                throw new Exception("An error occurred while saving school transaction.");
            }
        }
        public async Task<List<SchoolSummaryDto>> GetSchoolSummariesBySiteAsync(short site)
        {
            var schools = new List<SchoolSummaryDto>();

            using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
            using var command = new SqlCommand("dbo.Show_Schools_Summary", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@site", site);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var school = new SchoolSummaryDto
                {
                    SchoolID = reader.GetInt32(reader.GetOrdinal("SchoolID")),
                    CoachFullName = reader.GetString(reader.GetOrdinal("FullName")),
                    SchoolLevel = reader.GetString(reader.GetOrdinal("description")), // Will be duplicated, we resolve this in mapping
                    SchoolType = reader.GetString(reader.GetOrdinal("description1")),
                    Days = reader.GetString(reader.GetOrdinal("Dayes")),
                    FromTo = reader.GetString(reader.GetOrdinal("From / To")),
                    NumberVsCapacity = reader.GetString(reader.GetOrdinal("Number / Capacity"))
                };

                schools.Add(school);
            }

            return schools;
        }
    }
}