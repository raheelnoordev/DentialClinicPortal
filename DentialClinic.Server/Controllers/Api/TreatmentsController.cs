using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentsController : ControllerBase
    {
        private readonly ITreatmentService _service;

        public TreatmentsController(ITreatmentService service)
        {
            _service = service;
        }

        [HttpGet("GetAllTreatments")]
        public async Task<IActionResult> GetAllTreatments()
        {
            var result = await _service.GetAllTreatmentsAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetTreatmentById/{id}")]
        public async Task<IActionResult> GetTreatmentById(int id)
        {
            var result = await _service.GetTreatmentByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateTreatment")]
        public async Task<IActionResult> CreateTreatment([FromBody] DentialClinic.Server.Models.Treatment.TreatmentCatalogDto request)
        {
            var result = await _service.CreateTreatmentAsync(request);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetTreatmentById), new { id = result.Result?.TreatmentId }, result);
        }

        [HttpPut("UpdateTreatment/{id}")]
        public async Task<IActionResult> UpdateTreatment(int id, [FromBody] DentialClinic.Server.Models.Treatment.TreatmentCatalogDto request)
        {
            var result = await _service.UpdateTreatmentAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteTreatment/{id}")]
        public async Task<IActionResult> DeleteTreatment(int id)
        {
            var result = await _service.DeleteTreatmentAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetActiveTreatments")]
        public async Task<IActionResult> GetActiveTreatments()
        {
            var result = await _service.GetActiveTreatmentsAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}

