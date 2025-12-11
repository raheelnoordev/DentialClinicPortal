using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.Expense;
using DentialClinic.Server.Models.UserManagement;

namespace DentialClinic.Server.Models.Expense
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }
        
        [Required]
        public int BranchId { get; set; }
        
        [Required]
        public int ExpenseCategoryId { get; set; }
        
        [MaxLength(255)]
        public string? Description { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime ExpenseDate { get; set; }
        
        [MaxLength(150)]
        public string? PaidTo { get; set; }
        
        [MaxLength(30)]
        public string? PaymentMethod { get; set; } // CASH/BANK/OTHER
        
        public string? Notes { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int? CreatedByUserId { get; set; }

        // Navigation properties
        public virtual BranchDto Branch { get; set; } = null!;
        public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;
        public virtual Users? CreatedByUser { get; set; }
    }
}

