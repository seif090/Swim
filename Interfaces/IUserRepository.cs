using SwimmingAcademy.DTOs;
namespace SwimmingAcademy.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserSiteAsync(int userId, short site);
        Task<int> InsertUserAsync(InsertUserRequest request);
        Task<bool> UpdateUserAsync(UpdateUserRequest request);
        Task<List<UserSummaryDto>> GetUserSummaryBySiteAsync(short site);
        Task<UserDetailsDto> GetUserDetailsAsync(int userId);

    }
}
