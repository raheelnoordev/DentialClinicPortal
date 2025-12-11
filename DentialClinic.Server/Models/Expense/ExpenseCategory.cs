using System.ComponentModel.DataAnnotations;

namespace DentialClinic.Server.Models.Expense
{
    public class ExpenseCategory
    {
        [Key]
        public int ExpenseCategoryId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public bool IsSalaryCategory { get; set; } = false;

        // Navigation properties
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}

