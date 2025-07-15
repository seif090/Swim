namespace SwimmingAcademy.DTOs
{
    public class InsertCoachRequest
    {
        public int UserId { get; set; }
        public short Site { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public short Gender { get; set; } // 1 = Male, 2 = Female, etc.
        public bool IsActive { get; set; }
        public short Type { get; set; }   // CoachType: 8 = School, 9 = PreTeam, etc.
        public string MobileNumber { get; set; } = string.Empty;
    }
}
