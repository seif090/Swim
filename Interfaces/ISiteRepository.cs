using SwimmingAcademy.DTOs;

namespace SwimmingAcademy.Interfaces
{
    public interface ISiteRepository
    {
        Task<bool> InsertNewSiteAsync(InsertSiteRequest request);
        Task<List<SiteDto>> GetAllSitesAsync(int userId);

    }
}
