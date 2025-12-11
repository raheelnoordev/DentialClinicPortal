using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _service;

        public BillingController(IBillingService service)
        {
            _service = service;
        }

        [HttpGet("GetAllInvoices")]
        public async Task<IActionResult> GetAllInvoices()
        {
            var result = await _service.GetAllInvoicesAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetInvoiceById/{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var result = await _service.GetInvoiceByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateInvoiceFromVisit/{visitId}")]
        public async Task<IActionResult> CreateInvoiceFromVisit(int visitId)
        {
            var result = await _service.CreateInvoiceFromVisitAsync(visitId);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = result.Result?.InvoiceId }, result);
        }

        [HttpPut("UpdateInvoice/{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] DentialClinic.Server.Models.Billing.InvoiceDto request)
        {
            var result = await _service.UpdateInvoiceAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteInvoice/{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _service.DeleteInvoiceAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("AddPayment/{invoiceId}")]
        public async Task<IActionResult> AddPayment(int invoiceId, [FromBody] DentialClinic.Server.Models.Billing.InvoicePaymentDto payment)
        {
            var result = await _service.AddPaymentAsync(invoiceId, payment);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetInvoicesByBranch/{branchId}")]
        public async Task<IActionResult> GetInvoicesByBranch(int branchId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _service.GetInvoicesByBranchAsync(branchId, startDate, endDate);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetInvoicesByPatient/{patientId}")]
        public async Task<IActionResult> GetInvoicesByPatient(int patientId)
        {
            var result = await _service.GetInvoicesByPatientAsync(patientId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetOutstandingBalance/{invoiceId}")]
        public async Task<IActionResult> GetOutstandingBalance(int invoiceId)
        {
            var result = await _service.GetOutstandingBalanceAsync(invoiceId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}

