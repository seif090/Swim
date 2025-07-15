namespace SwimmingAcademy.DTOs
{
    public class SchoolOptionDto
    {
        public int SchoolID { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string Dayes { get; set; } = string.Empty;
        public string FromTo { get; set; } = string.Empty;
    }
}
