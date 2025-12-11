using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.Visit;
using DentialClinic.Server.Models.UserManagement;

namespace DentialClinic.Server.Models.Billing
{
    [Table("invoice")]
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }
        
        [Required]
        public int BranchId { get; set; }
        
        [Required]
        public int VisitId { get; set; }
        
        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal GrossAmount { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal DiscountAmount { get; set; } = 0;
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal NetAmount { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal PaidAmount { get; set; } = 0;
        
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "UNPAID"; // UNPAID/PARTIAL/PAID
        
        public int? CreatedByUserId { get; set; }

        // Navigation properties
        public virtual BranchDto Branch { get; set; } = null!;
        public virtual VisitDto Visit { get; set; } = null!;
        public virtual Users? CreatedByUser { get; set; }
        public virtual ICollection<InvoicePayment> Payments { get; set; } = new List<InvoicePayment>();
    }
}

