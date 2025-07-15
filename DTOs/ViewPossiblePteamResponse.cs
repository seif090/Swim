namespace SwimmingAcademy.DTOs
{
    public class ViewPossiblePteamResponse
    {
        public List<PTeamDto> PTeams { get; set; } = new();
        public List<InvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
