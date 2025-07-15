namespace SwimmingAcademy.DTOs
{
    public class SavePTeamTransResponse
    {
        public long SwimmerID { get; set; }
        public long PTeamID { get; set; }
        public string InvItem { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
