namespace SwimmingAcademy.DTOs
{
    public class UpdateCoachRequest
    {
        public int UserId { get; set; }
        public short Site { get; set; }
        public int CoachId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public short Type { get; set; } // Coach type (e.g. 8 = School, 9 = PreTeam)
        public bool IsActive { get; set; }
    }
}
