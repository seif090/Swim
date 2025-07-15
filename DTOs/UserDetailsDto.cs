namespace SwimmingAcademy.DTOs
{
    public class UserDetailsDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Group { get; set; }
        public List<string> Sites { get; set; } = new();
    }
}
