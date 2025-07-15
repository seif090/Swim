namespace SwimmingAcademy.DTOs
{
    public class InvoiceOptionDto
    {
        public string ItemName { get; set; } = string.Empty;
        public short DurationInMonths { get; set; }
        public decimal Amount { get; set; }
    }
}
