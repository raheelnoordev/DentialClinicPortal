using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _service;

        public ExpensesController(IExpenseService service)
        {
            _service = service;
        }

        // Expense Categories
        [HttpGet("GetAllExpenseCategories")]
        public async Task<IActionResult> GetAllExpenseCategories()
        {
            var result = await _service.GetAllExpenseCategoriesAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetExpenseCategoryById/{id}")]
        public async Task<IActionResult> GetExpenseCategoryById(int id)
        {
            var result = await _service.GetExpenseCategoryByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateExpenseCategory")]
        public async Task<IActionResult> CreateExpenseCategory([FromBody] DentialClinic.Server.Models.Expense.ExpenseCategory request)
        {
            var result = await _service.CreateExpenseCategoryAsync(request);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetExpenseCategoryById), new { id = result.Result?.ExpenseCategoryId }, result);
        }

        [HttpPut("UpdateExpenseCategory/{id}")]
        public async Task<IActionResult> UpdateExpenseCategory(int id, [FromBody] DentialClinic.Server.Models.Expense.ExpenseCategory request)
        {
            var result = await _service.UpdateExpenseCategoryAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteExpenseCategory/{id}")]
        public async Task<IActionResult> DeleteExpenseCategory(int id)
        {
            var result = await _service.DeleteExpenseCategoryAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Expenses
        [HttpGet("GetAllExpenses")]
        public async Task<IActionResult> GetAllExpenses()
        {
            var result = await _service.GetAllExpensesAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetExpenseById/{id}")]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            var result = await _service.GetExpenseByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateExpense")]
        public async Task<IActionResult> CreateExpense([FromBody] DentialClinic.Server.Models.Expense.ExpenseDto request)
        {
            var result = await _service.CreateExpenseAsync(request);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetExpenseById), new { id = result.Result?.ExpenseId }, result);
        }

        [HttpPut("UpdateExpense/{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] DentialClinic.Server.Models.Expense.ExpenseDto request)
        {
            var result = await _service.UpdateExpenseAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteExpense/{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var result = await _service.DeleteExpenseAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetExpensesByBranch/{branchId}")]
        public async Task<IActionResult> GetExpensesByBranch(int branchId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _service.GetExpensesByBranchAsync(branchId, startDate, endDate);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Salary Payments
        [HttpGet("GetAllSalaryPayments")]
        public async Task<IActionResult> GetAllSalaryPayments()
        {
            var result = await _service.GetAllSalaryPaymentsAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetSalaryPaymentById/{id}")]
        public async Task<IActionResult> GetSalaryPaymentById(int id)
        {
            var result = await _service.GetSalaryPaymentByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateSalaryPayment")]
        public async Task<IActionResult> CreateSalaryPayment([FromBody] DentialClinic.Server.Models.Expense.SalaryPaymentDto request)
        {
            var result = await _service.CreateSalaryPaymentAsync(request);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetSalaryPaymentById), new { id = result.Result?.SalaryPaymentId }, result);
        }

        [HttpPut("UpdateSalaryPayment/{id}")]
        public async Task<IActionResult> UpdateSalaryPayment(int id, [FromBody] DentialClinic.Server.Models.Expense.SalaryPaymentDto request)
        {
            var result = await _service.UpdateSalaryPaymentAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteSalaryPayment/{id}")]
        public async Task<IActionResult> DeleteSalaryPayment(int id)
        {
            var result = await _service.DeleteSalaryPaymentAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetSalaryPaymentsByBranch/{branchId}")]
        public async Task<IActionResult> GetSalaryPaymentsByBranch(int branchId, [FromQuery] int? month, [FromQuery] int? year)
        {
            var result = await _service.GetSalaryPaymentsByBranchAsync(branchId, month, year);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}

