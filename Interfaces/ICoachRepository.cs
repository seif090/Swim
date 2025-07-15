using SwimmingAcademy.DTOs;

namespace SwimmingAcademy.Interfaces
{
    public interface ICoachRepository
    {
        List<FreeCoachDto> GetFreeCoaches(FreeCoachFilterRequest request);
        Task<bool> InsertNewCoachAsync(InsertCoachRequest request);
        Task<bool> UpdateCoachAsync(UpdateCoachRequest request);
        Task<List<CoachesDto>> GetCoachesBySiteAsync(short site);

    }
}