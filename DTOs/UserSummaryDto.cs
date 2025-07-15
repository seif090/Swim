namespace SwimmingAcademy.DTOs
{
    public class UserSummaryDto
    {
        public int UserId { get; set; }
        public int NumberOfSites { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Active { get; set; } = string.Empty; // "True" or "False" as string
        public string Group { get; set; } = string.Empty;
    }
}
