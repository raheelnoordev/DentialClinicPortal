using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Expense;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;
        public ExpenseService(ApplicationDbContext context) => _context = context;

        // Expense Categories
        public async Task<ServiceResponse<List<ExpenseCategory>>> GetAllExpenseCategoriesAsync()
        {
            var response = new ServiceResponse<List<ExpenseCategory>>();
            try
            {
                response.Result = await _context.ExpenseCategories.OrderBy(ec => ec.Name).ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<ExpenseCategory>> GetExpenseCategoryByIdAsync(int id)
        {
            var response = new ServiceResponse<ExpenseCategory>();
            try
            {
                var category = await _context.ExpenseCategories.FindAsync(id);
                if (category == null) { response.Success = false; response.Message = "Category not found"; return response; }
                response.Result = category;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<ExpenseCategory>> CreateExpenseCategoryAsync(ExpenseCategory request)
        {
            var response = new ServiceResponse<ExpenseCategory>();
            try
            {
                _context.ExpenseCategories.Add(request);
                await _context.SaveChangesAsync();
                response.Result = request;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<ExpenseCategory>> UpdateExpenseCategoryAsync(int id, ExpenseCategory request)
        {
            var response = new ServiceResponse<ExpenseCategory>();
            try
            {
                var category = await _context.ExpenseCategories.FindAsync(id);
                if (category == null) { response.Success = false; response.Message = "Category not found"; return response; }
                category.Name = request.Name;
                category.IsSalaryCategory = request.IsSalaryCategory;
                await _context.SaveChangesAsync();
                response.Result = category;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteExpenseCategoryAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var category = await _context.ExpenseCategories.FindAsync(id);
                if (category == null) { response.Success = false; response.Message = "Category not found"; return response; }
                _context.ExpenseCategories.Remove(category);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        // Expenses
        public async Task<ServiceResponse<List<ExpenseDto>>> GetAllExpensesAsync()
        {
            var response = new ServiceResponse<List<ExpenseDto>>();
            try
            {
                response.Result = await _context.Expenses
                    .Include(e => e.Branch)
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.CreatedByUser)
                    .OrderByDescending(e => e.ExpenseDate)
                    .Select(e => new ExpenseDto
                    {
                        ExpenseId = e.ExpenseId,
                        BranchId = e.BranchId,
                        BranchName = e.Branch.Name,
                        ExpenseCategoryId = e.ExpenseCategoryId,
                        ExpenseCategoryName = e.ExpenseCategory.Name,
                        Description = e.Description,
                        Amount = e.Amount,
                        ExpenseDate = e.ExpenseDate,
                        PaidTo = e.PaidTo,
                        PaymentMethod = e.PaymentMethod,
                        Notes = e.Notes,
                        CreatedAt = e.CreatedAt,
                        CreatedByUserId = e.CreatedByUserId,
                        CreatedByUserName = e.CreatedByUser != null ? e.CreatedByUser.FullName : null
                    })
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<ExpenseDto>> GetExpenseByIdAsync(int id)
        {
            var response = new ServiceResponse<ExpenseDto>();
            try
            {
                var expense = await _context.Expenses
                    .Include(e => e.Branch)
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.CreatedByUser)
                    .Where(e => e.ExpenseId == id)
                    .Select(e => new ExpenseDto
                    {
                        ExpenseId = e.ExpenseId,
                        BranchId = e.BranchId,
                        BranchName = e.Branch.Name,
                        ExpenseCategoryId = e.ExpenseCategoryId,
                        ExpenseCategoryName = e.ExpenseCategory.Name,
                        Description = e.Description,
                        Amount = e.Amount,
                        ExpenseDate = e.ExpenseDate,
                        PaidTo = e.PaidTo,
                        PaymentMethod = e.PaymentMethod,
                        Notes = e.Notes,
                        CreatedAt = e.CreatedAt,
                        CreatedByUserId = e.CreatedByUserId,
                        CreatedByUserName = e.CreatedByUser != null ? e.CreatedByUser.FullName : null
                    })
                    .FirstOrDefaultAsync();
                if (expense == null) { response.Success = false; response.Message = "Expense not found"; return response; }
                response.Result = expense;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<ExpenseDto>> CreateExpenseAsync(ExpenseDto request)
        {
            var response = new ServiceResponse<ExpenseDto>();
            try
            {
                var expense = new Expense
                {
                    BranchId = request.BranchId,
                    ExpenseCategoryId = request.ExpenseCategoryId,
                    Description = request.Description,
                    Amount = request.Amount,
                    ExpenseDate = request.ExpenseDate,
                    PaidTo = request.PaidTo,
                    PaymentMethod = request.PaymentMethod,
                    Notes = request.Notes,
                    CreatedAt = DateTime.Now,
                    CreatedByUserId = request.CreatedByUserId
                };
                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();
                var getResponse = await GetExpenseByIdAsync(expense.ExpenseId);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<ExpenseDto>> UpdateExpenseAsync(int id, ExpenseDto request)
        {
            var response = new ServiceResponse<ExpenseDto>();
            try
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense == null) { response.Success = false; response.Message = "Expense not found"; return response; }
                expense.ExpenseCategoryId = request.ExpenseCategoryId;
                expense.Description = request.Description;
                expense.Amount = request.Amount;
                expense.ExpenseDate = request.ExpenseDate;
                expense.PaidTo = request.PaidTo;
                expense.PaymentMethod = request.PaymentMethod;
                expense.Notes = request.Notes;
                await _context.SaveChangesAsync();
                var getResponse = await GetExpenseByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteExpenseAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense == null) { response.Success = false; response.Message = "Expense not found"; return response; }
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<List<ExpenseDto>>> GetExpensesByBranchAsync(int branchId, DateTime? startDate, DateTime? endDate)
        {
            var response = new ServiceResponse<List<ExpenseDto>>();
            try
            {
                var query = _context.Expenses
                    .Include(e => e.Branch)
                    .Include(e => e.ExpenseCategory)
                    .Include(e => e.CreatedByUser)
                    .Where(e => e.BranchId == branchId);
                if (startDate.HasValue) query = query.Where(e => e.ExpenseDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(e => e.ExpenseDate <= endDate.Value);
                response.Result = await query
                    .OrderByDescending(e => e.ExpenseDate)
                    .Select(e => new ExpenseDto
                    {
                        ExpenseId = e.ExpenseId,
                        BranchId = e.BranchId,
                        BranchName = e.Branch.Name,
                        ExpenseCategoryId = e.ExpenseCategoryId,
                        ExpenseCategoryName = e.ExpenseCategory.Name,
                        Description = e.Description,
                        Amount = e.Amount,
                        ExpenseDate = e.ExpenseDate,
                        PaidTo = e.PaidTo,
                        PaymentMethod = e.PaymentMethod,
                        Notes = e.Notes,
                        CreatedAt = e.CreatedAt,
                        CreatedByUserId = e.CreatedByUserId,
                        CreatedByUserName = e.CreatedByUser != null ? e.CreatedByUser.FullName : null
                    })
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        // Salary Payments
        public async Task<ServiceResponse<List<SalaryPaymentDto>>> GetAllSalaryPaymentsAsync()
        {
            var response = new ServiceResponse<List<SalaryPaymentDto>>();
            try
            {
                response.Result = await _context.SalaryPayments
                    .Include(sp => sp.Branch)
                    .Include(sp => sp.User)
                    .OrderByDescending(sp => sp.PaymentDate)
                    .Select(sp => new SalaryPaymentDto
                    {
                        SalaryPaymentId = sp.SalaryPaymentId,
                        BranchId = sp.BranchId,
                        BranchName = sp.Branch.Name,
                        UserId = sp.UserId,
                        UserName = sp.User.FullName,
                        Month = sp.Month,
                        Year = sp.Year,
                        BasicAmount = sp.BasicAmount,
                        BonusAmount = sp.BonusAmount,
                        Deductions = sp.Deductions,
                        NetPaidAmount = sp.NetPaidAmount,
                        PaymentDate = sp.PaymentDate,
                        PaymentMethod = sp.PaymentMethod,
                        Notes = sp.Notes
                    })
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<SalaryPaymentDto>> GetSalaryPaymentByIdAsync(int id)
        {
            var response = new ServiceResponse<SalaryPaymentDto>();
            try
            {
                var payment = await _context.SalaryPayments
                    .Include(sp => sp.Branch)
                    .Include(sp => sp.User)
                    .Where(sp => sp.SalaryPaymentId == id)
                    .Select(sp => new SalaryPaymentDto
                    {
                        SalaryPaymentId = sp.SalaryPaymentId,
                        BranchId = sp.BranchId,
                        BranchName = sp.Branch.Name,
                        UserId = sp.UserId,
                        UserName = sp.User.FullName,
                        Month = sp.Month,
                        Year = sp.Year,
                        BasicAmount = sp.BasicAmount,
                        BonusAmount = sp.BonusAmount,
                        Deductions = sp.Deductions,
                        NetPaidAmount = sp.NetPaidAmount,
                        PaymentDate = sp.PaymentDate,
                        PaymentMethod = sp.PaymentMethod,
                        Notes = sp.Notes
                    })
                    .FirstOrDefaultAsync();
                if (payment == null) { response.Success = false; response.Message = "Salary payment not found"; return response; }
                response.Result = payment;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<SalaryPaymentDto>> CreateSalaryPaymentAsync(SalaryPaymentDto request)
        {
            var response = new ServiceResponse<SalaryPaymentDto>();
            try
            {
                var payment = new SalaryPayment
                {
                    BranchId = request.BranchId,
                    UserId = request.UserId,
                    Month = request.Month,
                    Year = request.Year,
                    BasicAmount = request.BasicAmount,
                    BonusAmount = request.BonusAmount,
                    Deductions = request.Deductions,
                    NetPaidAmount = request.NetPaidAmount,
                    PaymentDate = request.PaymentDate,
                    PaymentMethod = request.PaymentMethod,
                    Notes = request.Notes
                };
                _context.SalaryPayments.Add(payment);
                await _context.SaveChangesAsync();
                var getResponse = await GetSalaryPaymentByIdAsync(payment.SalaryPaymentId);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<SalaryPaymentDto>> UpdateSalaryPaymentAsync(int id, SalaryPaymentDto request)
        {
            var response = new ServiceResponse<SalaryPaymentDto>();
            try
            {
                var payment = await _context.SalaryPayments.FindAsync(id);
                if (payment == null) { response.Success = false; response.Message = "Salary payment not found"; return response; }
                payment.BasicAmount = request.BasicAmount;
                payment.BonusAmount = request.BonusAmount;
                payment.Deductions = request.Deductions;
                payment.NetPaidAmount = request.NetPaidAmount;
                payment.PaymentDate = request.PaymentDate;
                payment.PaymentMethod = request.PaymentMethod;
                payment.Notes = request.Notes;
                await _context.SaveChangesAsync();
                var getResponse = await GetSalaryPaymentByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteSalaryPaymentAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var payment = await _context.SalaryPayments.FindAsync(id);
                if (payment == null) { response.Success = false; response.Message = "Salary payment not found"; return response; }
                _context.SalaryPayments.Remove(payment);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<List<SalaryPaymentDto>>> GetSalaryPaymentsByBranchAsync(int branchId, int? month, int? year)
        {
            var response = new ServiceResponse<List<SalaryPaymentDto>>();
            try
            {
                var query = _context.SalaryPayments
                    .Include(sp => sp.Branch)
                    .Include(sp => sp.User)
                    .Where(sp => sp.BranchId == branchId);
                if (month.HasValue) query = query.Where(sp => sp.Month == month.Value);
                if (year.HasValue) query = query.Where(sp => sp.Year == year.Value);
                response.Result = await query
                    .OrderByDescending(sp => sp.PaymentDate)
                    .Select(sp => new SalaryPaymentDto
                    {
                        SalaryPaymentId = sp.SalaryPaymentId,
                        BranchId = sp.BranchId,
                        BranchName = sp.Branch.Name,
                        UserId = sp.UserId,
                        UserName = sp.User.FullName,
                        Month = sp.Month,
                        Year = sp.Year,
                        BasicAmount = sp.BasicAmount,
                        BonusAmount = sp.BonusAmount,
                        Deductions = sp.Deductions,
                        NetPaidAmount = sp.NetPaidAmount,
                        PaymentDate = sp.PaymentDate,
                        PaymentMethod = sp.PaymentMethod,
                        Notes = sp.Notes
                    })
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }
    }
}

