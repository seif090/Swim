namespace SwimmingAcademy.DTOs
{
    public class ViewPossibleSchoolResultDto
    {
        public List<SchoolOptionDto> SchoolOptions { get; set; } = new();
        public List<InvoiceOptionDto> InvoiceOptions { get; set; } = new();
    }
}
