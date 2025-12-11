using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.Billing;
using DentialClinic.Server.Models.UserManagement;

namespace DentialClinic.Server.Models.Billing
{
    public class InvoicePayment
    {
        [Key]
        public int PaymentId { get; set; }
        
        [Required]
        public int InvoiceId { get; set; }
        
        [Required]
        public int BranchId { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [MaxLength(30)]
        public string Method { get; set; } = string.Empty; // CASH/CARD/BANK/ONLINE
        
        [MaxLength(100)]
        public string? ReferenceNo { get; set; }
        
        public string? Notes { get; set; }
        
        public int? ReceivedByUserId { get; set; }

        // Navigation properties
        public virtual Invoice Invoice { get; set; } = null!;
        public virtual BranchDto Branch { get; set; } = null!;
        public virtual Users? ReceivedByUser { get; set; }
    }
}

