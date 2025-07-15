using Microsoft.Data.SqlClient;
using SwimmingAcademy.DTOs;
using SwimmingAcademy.Interfaces;
using System.Data;

namespace SwimmingAcademy.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task AddUserSiteAsync(int userId, short site)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlCommand command = new SqlCommand("dbo.AddSite_User", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@site", site);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task<int> InsertUserAsync(InsertUserRequest request)
        {
            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand command = new SqlCommand("dbo.insert_User", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@FullName", request.FullName);
            command.Parameters.AddWithValue("@site", request.Site);
            command.Parameters.AddWithValue("@disabled", request.Disabled);
            command.Parameters.AddWithValue("@userTypeID", request.UserTypeId);
            command.Parameters.AddWithValue("@Password", request.Password);

            await connection.OpenAsync();

            object result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result); // This is the returned UserID
        }
        public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
        {
            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand command = new SqlCommand("dbo.update_User", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Userid", request.UserId);
            command.Parameters.AddWithValue("@FullName", request.FullName);
            command.Parameters.AddWithValue("@site", request.Site);
            command.Parameters.AddWithValue("@disabled", request.Disabled);
            command.Parameters.AddWithValue("@userTypeID", request.UserTypeId);
            command.Parameters.AddWithValue("@Password", request.Password);

            await connection.OpenAsync();
            int affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }
        public async Task<List<UserSummaryDto>> GetUserSummaryBySiteAsync(short site)
        {
            var users = new List<UserSummaryDto>();

            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new SqlCommand("dbo.View_summary_Users", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Site", site);

            await conn.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new UserSummaryDto
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                    NumberOfSites = reader.GetInt32(reader.GetOrdinal("Number Of Sites")),
                    FullName = reader.GetString(reader.GetOrdinal("fullname")),
                    Active = reader.GetString(reader.GetOrdinal("Active")),
                    Group = reader.IsDBNull(reader.GetOrdinal("Group")) ? null : reader.GetString(reader.GetOrdinal("Group"))
                });
            }

            return users;
        }
        public async Task<UserDetailsDto> GetUserDetailsAsync(int userId)
        {
            var user = new UserDetailsDto();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("dbo.View_User", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@userID", userId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            // First result set: User info
            if (await reader.ReadAsync())
            {
                user.UserId = reader.GetInt32(reader.GetOrdinal("userid"));
                user.FullName = reader.GetString(reader.GetOrdinal("fullname"));
                user.Password = reader.GetString(reader.GetOrdinal("Password"));
                user.Group = reader.GetString(reader.GetOrdinal("description"));
            }

            // Move to second result set: Sites
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    string siteDescription = reader.GetString(reader.GetOrdinal("Sites"));
                    user.Sites.Add(siteDescription);
                }
            }

            return user;
        }
    }
}
