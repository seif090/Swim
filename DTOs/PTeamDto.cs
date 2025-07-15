namespace SwimmingAcademy.DTOs
{
    public class PTeamDto
    {
        public long PTeamID { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string Dayes { get; set; } = string.Empty;
        public string FromTo { get; set; } = string.Empty;
    }
}
