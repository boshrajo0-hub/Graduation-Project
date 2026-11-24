namespace HealthCareCenter.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int VisitId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "Unpaid";
        public DateTime? PaymentDate { get; set; }

        public Visit Visit { get; set; } = null!;
    }
}
