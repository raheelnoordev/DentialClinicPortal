using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Expense;

namespace DentialClinic.Server.Interfaces
{
    public interface IExpenseService
    {
        // Expense Categories
        Task<ServiceResponse<List<ExpenseCategory>>> GetAllExpenseCategoriesAsync();
        Task<ServiceResponse<ExpenseCategory>> GetExpenseCategoryByIdAsync(int id);
        Task<ServiceResponse<ExpenseCategory>> CreateExpenseCategoryAsync(ExpenseCategory request);
        Task<ServiceResponse<ExpenseCategory>> UpdateExpenseCategoryAsync(int id, ExpenseCategory request);
        Task<ServiceResponse<bool>> DeleteExpenseCategoryAsync(int id);
        
        // Expenses
        Task<ServiceResponse<List<ExpenseDto>>> GetAllExpensesAsync();
        Task<ServiceResponse<ExpenseDto>> GetExpenseByIdAsync(int id);
        Task<ServiceResponse<ExpenseDto>> CreateExpenseAsync(ExpenseDto request);
        Task<ServiceResponse<ExpenseDto>> UpdateExpenseAsync(int id, ExpenseDto request);
        Task<ServiceResponse<bool>> DeleteExpenseAsync(int id);
        Task<ServiceResponse<List<ExpenseDto>>> GetExpensesByBranchAsync(int branchId, DateTime? startDate, DateTime? endDate);
        
        // Salary Payments
        Task<ServiceResponse<List<SalaryPaymentDto>>> GetAllSalaryPaymentsAsync();
        Task<ServiceResponse<SalaryPaymentDto>> GetSalaryPaymentByIdAsync(int id);
        Task<ServiceResponse<SalaryPaymentDto>> CreateSalaryPaymentAsync(SalaryPaymentDto request);
        Task<ServiceResponse<SalaryPaymentDto>> UpdateSalaryPaymentAsync(int id, SalaryPaymentDto request);
        Task<ServiceResponse<bool>> DeleteSalaryPaymentAsync(int id);
        Task<ServiceResponse<List<SalaryPaymentDto>>> GetSalaryPaymentsByBranchAsync(int branchId, int? month, int? year);
    }
}

