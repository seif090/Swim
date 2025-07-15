using SwimmingAcademy.DTOs;

namespace SwimmingAcademy.Interfaces
{
    public interface IPreTeamRepository
    {
        CreatePreTeamResponse CreatePreTeam(CreatePTeamRequest request);
        Task<IEnumerable<PTeamSearchResultDto>> SearchPTeamAsync(PTeamSearchRequest request);
        Task<IEnumerable<SwimmerPTeamDetailsDto>> GetSwimmerPTeamDetailsAsync(long pTeamId);
        Task<bool> UpdatePTeamAsync(UpdatePTeamRequest request);
        Task<bool> EndPTeamAsync(EndPreTeamRequest request);
        Task<PTeamDetailsTabDto?> GetPTeamDetailsTabAsync(long pTeamId);
        Task<IEnumerable<ActionNameDto>> SearchActionsAsync(PreTeamActionSearchRequest request);
        Task<SavePTeamTransResponse?> SavePTeamTransactionAsync(SavePteamTransRequest request);
        Task<ViewPossiblePteamResponse> ViewPossiblePteamAsync(long swimmerId);

    }
}
