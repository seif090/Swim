using SwimmingAcademy.DTOs;

namespace SwimmingAcademy.Interfaces
{
    public interface ISchoolRepository
    {
        CreateSchoolResponse CreateSchool(CreateSchoolRequest request);
        Task<IEnumerable<SchoolSearchResultDto>> SearchSchoolsAsync(SchoolSearchRequest request);
        Task<bool> UpdateSchoolAsync(UpdateSchoolRequest request);
        Task<bool> EndSchoolAsync(EndSchoolRequest request);
        Task<SchoolDetailsTabDto?> GetSchoolDetailsTabAsync(long schoolID);
        Task<IEnumerable<SchoolSwimmerDetailsDto>> GetSchoolSwimmerDetailsAsync(long schoolID);
        Task<IEnumerable<ActionNameDto>> SearchSchoolActionsAsync(SchoolActionSearchRequest request);
        Task<ViewPossibleSchoolResponse> ViewPossibleSchoolAsync(long swimmerId, short type);
        Task<SaveSchoolTransResponse?> SaveSchoolTransactionAsync(SaveSchoolTransRequest request);
        Task<List<SchoolSummaryDto>> GetSchoolSummariesBySiteAsync(short site);


    }
}
