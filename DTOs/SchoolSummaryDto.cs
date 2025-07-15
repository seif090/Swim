namespace SwimmingAcademy.DTOs
{
    public class SchoolSummaryDto
    {
        public int SchoolID { get; set; }
        public string CoachFullName { get; set; }
        public string SchoolLevel { get; set; }
        public string SchoolType { get; set; }
        public string Days { get; set; }
        public string FromTo { get; set; }
        public string NumberVsCapacity { get; set; }
    }
}
