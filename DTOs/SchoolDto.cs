namespace SwimmingAcademy.DTOs
{
    public class SchoolDto
    {
        public long SchoolID { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string Days { get; set; } = string.Empty;
        public string FromTo { get; set; } = string.Empty;
    }
}
