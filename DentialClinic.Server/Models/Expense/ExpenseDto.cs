using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Expense
{
    [NotMapped]
    public class ExpenseDto
    {
        public int ExpenseId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int ExpenseCategoryId { get; set; }
        public string ExpenseCategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? PaidTo { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
    }

    public class SalaryPaymentDto
    {
        public int SalaryPaymentId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }
}

