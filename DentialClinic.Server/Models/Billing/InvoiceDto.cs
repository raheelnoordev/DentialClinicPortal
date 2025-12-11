using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Billing
{
    [NotMapped]
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int VisitId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string Status { get; set; } = "UNPAID";
        public int? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public List<InvoicePaymentDto> Payments { get; set; } = new List<InvoicePaymentDto>();
    }

    public class InvoicePaymentDto
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? ReferenceNo { get; set; }
        public string? Notes { get; set; }
        public int? ReceivedByUserId { get; set; }
        public string? ReceivedByUserName { get; set; }
    }
}

