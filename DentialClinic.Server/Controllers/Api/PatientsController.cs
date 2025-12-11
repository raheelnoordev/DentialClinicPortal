using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet("GetAllPatients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var result = await _service.GetAllPatientsAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetPatientById/{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var result = await _service.GetPatientByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreatePatient")]
        public async Task<IActionResult> CreatePatient([FromBody] DentialClinic.Server.Models.Patient.PatientDto request)
        {
            var result = await _service.CreatePatientAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("UpdatePatient/{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] DentialClinic.Server.Models.Patient.PatientDto request)
        {
            var result = await _service.UpdatePatientAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeletePatient/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _service.DeletePatientAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}

