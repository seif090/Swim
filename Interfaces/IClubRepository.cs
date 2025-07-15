using SwimmingAcademy.DTOs;

namespace SwimmingAcademy.Interfaces
{
    public interface IClubRepository
    {
        Task<bool> InsertNewClubAsync(InsertClubRequest request);
        Task<List<ClubDto>> GetAllClubsAsync(int userId);

    }
}
