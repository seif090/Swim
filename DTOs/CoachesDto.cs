namespace SwimmingAcademy.DTOs
{
    public class CoachesDto
    {
        public int CoachID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string CoachType { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
    }
}
