﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwimmingAcademy.Data;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using System.Data;

namespace SwimmingAcademy.Repositories
{
    public class SwimmerRepository : ISwimmerRepository
    {
        private readonly SwimmingAcademyContext _context;
        private readonly ILogger<SwimmerRepository> _logger;

        public SwimmerRepository(SwimmingAcademyContext context, ILogger<SwimmerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<long> AddSwimmerAsync(AddSwimmerRequestDTO req)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[Add_Swimmer]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(new[]
                {
            new SqlParameter("@UserID", req.UserID),
            new SqlParameter("@Site", req.Site),
            new SqlParameter("@FullName", req.FullName),
            new SqlParameter("@BirthDate", req.BirthDate),
            new SqlParameter("@Start_Level", req.StartLevel),
            new SqlParameter("@Gender", req.Gender),
            new SqlParameter("@club", req.Club),
            new SqlParameter("@primaryPhone", req.PrimaryPhone),
            new SqlParameter("@SecondaryPhone", (object?)req.SecondaryPhone ?? DBNull.Value),
            new SqlParameter("@PrimaryJop", req.PrimaryJob),
            new SqlParameter("@SecondaryJop", (object?)req.SecondaryJob ?? DBNull.Value),
            new SqlParameter("@Email", req.Email),
            new SqlParameter("@Adress", req.Address)
        });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                var result = await command.ExecuteScalarAsync();
                return result is null ? 0 : Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddSwimmerAsync");
                throw new Exception("An error occurred while adding the swimmer.");
            }
        }

        public async Task<SwimmerInfoTabDto?> GetSwimmerInfoTabAsync(long swimmerID)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Swimmers].[InfoTap]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@SwimmerID", swimmerID));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new SwimmerInfoTabDto
                    {
                        FullName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        BirthDate = reader.IsDBNull(1) ? default : reader.GetDateTime(1),
                        Site = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        CurrentLevel = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        StartLevel = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        CreatedAtDate = reader.IsDBNull(5) ? default : reader.GetDateTime(5),
                        PrimaryJop = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        SecondaryJop = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        PrimaryPhone = reader.IsDBNull(8) ? "" : reader.GetString(8),
                        SecondaryPhone = reader.IsDBNull(9) ? "" : reader.GetString(9),
                        Club = reader.IsDBNull(10) ? "" : reader.GetString(10)
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSwimmerInfoTabAsync");
                throw new Exception("An error occurred while retrieving swimmer info.");
            }
        }
        public async Task<IEnumerable<SwimmerLogDto>> GetSwimmerLogsAsync(long swimmerID)
        {
            var logs = new List<SwimmerLogDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[LogTap]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@SwimmerID", swimmerID));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    logs.Add(new SwimmerLogDto
                    {
                        ActionName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        PerformedBy = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        CreatedAtDate = reader.IsDBNull(2) ? default : reader.GetDateTime(2),
                        Site = reader.IsDBNull(3) ? "" : reader.GetString(3)
                    });
                }

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSwimmerLogsAsync");
                throw new Exception("An error occurred while retrieving swimmer logs.");
            }
        }

        public async Task<long> ChangeSwimmerSiteAsync(ChangeSwimmerSiteRequest request)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[Change_Site]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@swimmerID", request.SwimmerID));
                command.Parameters.Add(new SqlParameter("@userID", request.UserID));
                command.Parameters.Add(new SqlParameter("@Site", request.Site));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                var result = await command.ExecuteScalarAsync();
                return result is null ? 0 : Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing swimmer site for SwimmerID {SwimmerID}", request.SwimmerID);
                throw new Exception("An error occurred while changing the swimmer's site.");
            }
        }

        public async Task<bool> DeleteSwimmerAsync(long swimmerID)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[drop_Swimmer]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@swimmerID", swimmerID));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteSwimmerAsync");
                throw new Exception("An error occurred while deleting swimmer.");
            }
        }

        

       
        public async Task<IEnumerable<ActionNameDto>> SearchSwimmerActionsAsync(SwimmerActionSearchRequest req)
        {
            var actions = new List<ActionNameDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[SearchActions]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@UserID", req.UserID));
                command.Parameters.Add(new SqlParameter("@SwimmerID", req.SwimmerID));
                command.Parameters.Add(new SqlParameter("@userSite", req.UserSite));

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
                _logger.LogError(ex, "Error in SearchSwimmerActionsAsync");
                throw new Exception("An error occurred while searching swimmer actions.");
            }
        }

        public async Task<IEnumerable<SwimmerSearchResultDto>> SearchSwimmersAsync(SwimmerSearchRequest req)
        {
            var result = new List<SwimmerSearchResultDto>();
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();
                command.CommandText = "[Swimmers].[ShowSwimmers]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@swimmerID", (object?)req.SwimmerID ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@FullName", (object?)req.FullName ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@year", (object?)req.Year ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@level", (object?)req.Level ?? DBNull.Value));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new SwimmerSearchResultDto
                    {
                        FullName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Year = reader.IsDBNull(1) ? "" : reader.GetValue(1)?.ToString() ?? "",
                        CurrentLevel = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        CoachName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Site = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        Club = reader.IsDBNull(5) ? "" : reader.GetString(5)
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchSwimmersAsync");
                throw new Exception("An error occurred while searching swimmers.");
            }
        }

        public async Task<object?> GetTechnicalTapAsync(long swimmerID)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[TechnicalTap]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@swimmerID", swimmerID));

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    if (reader.FieldCount == 7) // School shape
                    {
                        return new SwimmerTechnicalSchoolDto
                        {
                            CoachName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            FirstDay = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            SecondDay = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            StartTime = reader.IsDBNull(3) ? TimeSpan.Zero : TimeSpan.Parse(reader.GetValue(3)?.ToString() ?? ""),
                            EndTime = reader.IsDBNull(4) ? TimeSpan.Zero : TimeSpan.Parse(reader.GetValue(4)?.ToString() ?? ""),
                            SwimmerLevel = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            Attendence = reader.IsDBNull(6) ? "" : reader.GetString(6)
                        };
                    }
                    else if (reader.FieldCount == 9) // PreTeam shape
                    {
                        return new SwimmerTechnicalPreTeamDto
                        {
                            CoachName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            FirstDay = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            SecondDay = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            ThirdDay = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            StartTime = reader.IsDBNull(4) ? TimeSpan.Zero : TimeSpan.Parse(reader.GetValue(4)?.ToString() ?? ""),
                            EndTime = reader.IsDBNull(5) ? TimeSpan.Zero : TimeSpan.Parse(reader.GetValue(5)?.ToString() ?? ""),
                            SwimmerLevel = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            Attendence = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            LastStar = reader.IsDBNull(8) ? "" : reader.GetString(8)
                        };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTechnicalTapAsync");
                throw new Exception("An error occurred while retrieving technical tab.");
            }
        }

        public async Task<bool> UpdateSwimmerAsync(UpdateSwimmerRequest req)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[Update_Swimmer]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(new[]
                {
                    new SqlParameter("@swimmerID", req.SwimmerID),
                    new SqlParameter("@UserID", req.UserID),
                    new SqlParameter("@Site", req.Site),
                    new SqlParameter("@FullName", req.FullName),
                    new SqlParameter("@BirthDate", req.BirthDate),
                    new SqlParameter("@Gender", req.Gender),
                    new SqlParameter("@club", req.Club),
                    new SqlParameter("@primaryPhone", req.PrimaryPhone),
                    new SqlParameter("@SecondaryPhone", (object?)req.SecondaryPhone ?? DBNull.Value),
                    new SqlParameter("@PrimaryJop", req.PrimaryJop),
                    new SqlParameter("@SecondaryJop", (object?)req.SecondaryJop ?? DBNull.Value),
                    new SqlParameter("@Email", req.Email),
                    new SqlParameter("@Adress", req.Adress)
                });

                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateSwimmerAsync");
                throw new Exception("An error occurred while updating swimmer.");
            }
        }

        public async Task<bool> UpdateSwimmerLevelAsync(UpdateSwimmerLevelRequest req)
        {
            try
            {
                using var conn = _context.Database.GetDbConnection();
                using var command = conn.CreateCommand();

                command.CommandText = "[Swimmers].[UpdateSwimmerLevel]";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddRange(new[]
                {
                    new SqlParameter("@swimmerID", req.SwimmerID),
                    new SqlParameter("@NewLevel", req.NewLevel),
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
                _logger.LogError(ex, "Error in UpdateSwimmerLevelAsync");
                throw new Exception("An error occurred while updating swimmer level.");
            }
        }

       
    }
}