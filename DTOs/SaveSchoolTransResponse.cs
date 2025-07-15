using SwimmingAcademy.Models;
using System.Data;

namespace SwimmingAcademy.DTOs
{
    public class SaveSchoolTransResponse
    {
        public long SwimmerID { get; set; }
        public long SchoolID { get; set; }
        public string InvItem { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
