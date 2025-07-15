namespace SwimmingAcademy.DTOs
{
    public class ViewPossiblePreTeamResultDto
    {
        public long SwimmerID { get; set; }
        public List<PreTeamOptionDto> PTeams { get; set; } = new();
        public List<InvoiceOptionDto> Invoices { get; set; } = new();
    }
}
