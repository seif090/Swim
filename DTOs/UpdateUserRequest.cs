namespace SwimmingAcademy.DTOs
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public short Site { get; set; }
        public bool Disabled { get; set; }
        public short UserTypeId { get; set; }
        public string Password { get; set; }
    }
}
