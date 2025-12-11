using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models.Appointment;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments(
            [FromQuery] int? branchId = null,
            [FromQuery] int? doctorId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            var result = await _service.GetAllAppointmentsAsync(branchId, doctorId, status, dateFrom, dateTo);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var result = await _service.GetAppointmentByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDto request)
        {
            var result = await _service.CreateAppointmentAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDto request)
        {
            var result = await _service.UpdateAppointmentAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromQuery] string status)
        {
            var result = await _service.UpdateAppointmentStatusAsync(id, status);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var result = await _service.DeleteAppointmentAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}

