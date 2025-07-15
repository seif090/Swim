namespace SwimmingAcademy.DTOs
{
    /// <summary>
    /// Represents the result of an invoice transaction for a swimmer.
    /// </summary>
    public class InvoiceResultDto
    {
        public string InvItem { get; set; } = string.Empty;  
        public decimal Value { get; set; }
    }
}
