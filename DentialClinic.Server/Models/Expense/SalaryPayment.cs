using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.UserManagement;

namespace DentialClinic.Server.Models.Expense
{
    public class SalaryPayment
    {
        [Key]
        public int SalaryPaymentId { get; set; }
        
        [Required]
        public int BranchId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [Range(1, 12)]
        public int Month { get; set; }
        
        [Required]
        public int Year { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal BasicAmount { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal BonusAmount { get; set; } = 0;
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Deductions { get; set; } = 0;
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal NetPaidAmount { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; }
        
        [MaxLength(30)]
        public string? PaymentMethod { get; set; } // CASH/BANK
        
        public string? Notes { get; set; }

        // Navigation properties
        public virtual BranchDto Branch { get; set; } = null!;
        public virtual Users User { get; set; } = null!;
    }
}

