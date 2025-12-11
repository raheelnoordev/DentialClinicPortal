using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitsController : ControllerBase
    {
        private readonly IVisitService _service;

        public VisitsController(IVisitService service)
        {
            _service = service;
        }

        [HttpGet("GetAllVisits")]
        public async Task<IActionResult> GetAllVisits()
        {
            var result = await _service.GetAllVisitsAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetVisitById/{id}")]
        public async Task<IActionResult> GetVisitById(int id)
        {
            var result = await _service.GetVisitByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateVisit")]
        public async Task<IActionResult> CreateVisit([FromBody] DentialClinic.Server.Models.Visit.VisitDto request)
        {
            var result = await _service.CreateVisitAsync(request);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetVisitById), new { id = result.Result?.VisitId }, result);
        }

        [HttpPut("UpdateVisit/{id}")]
        public async Task<IActionResult> UpdateVisit(int id, [FromBody] DentialClinic.Server.Models.Visit.VisitDto request)
        {
            var result = await _service.UpdateVisitAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteVisit/{id}")]
        public async Task<IActionResult> DeleteVisit(int id)
        {
            var result = await _service.DeleteVisitAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetVisitsByBranch/{branchId}")]
        public async Task<IActionResult> GetVisitsByBranch(int branchId, [FromQuery] DateTime? date)
        {
            var result = await _service.GetVisitsByBranchAsync(branchId, date);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetVisitsByPatient/{patientId}")]
        public async Task<IActionResult> GetVisitsByPatient(int patientId)
        {
            var result = await _service.GetVisitsByPatientAsync(patientId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("CloseVisit/{id}")]
        public async Task<IActionResult> CloseVisit(int id)
        {
            var result = await _service.CloseVisitAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}

