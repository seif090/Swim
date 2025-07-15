namespace SwimmingAcademy.DTOs
{
    public class ViewPossibleSchoolResponse
    {
        public List<SchoolDto> Schools { get; set; } = new();
        public List<InvoiceItemDto> InvoiceItems { get; set; } = new();
    }
}
