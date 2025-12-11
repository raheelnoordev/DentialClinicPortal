using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsAppController : ControllerBase
    {
        private readonly IWhatsAppService _service;

        public WhatsAppController(IWhatsAppService service)
        {
            _service = service;
        }

        // Settings
        [HttpGet("GetAllSettings")]
        public async Task<IActionResult> GetAllSettings()
        {
            var result = await _service.GetAllSettingsAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetSettingsByBranch/{branchId}")]
        public async Task<IActionResult> GetSettingsByBranch(int branchId)
        {
            var result = await _service.GetSettingsByBranchAsync(branchId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateOrUpdateSettings")]
        public async Task<IActionResult> CreateOrUpdateSettings([FromBody] DentialClinic.Server.Models.WhatsApp.WhatsAppSettings request)
        {
            var result = await _service.CreateOrUpdateSettingsAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Screens
        [HttpGet("GetAllScreens")]
        public async Task<IActionResult> GetAllScreens([FromQuery] int? branchId)
        {
            var result = await _service.GetAllScreensAsync(branchId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetScreenById/{id}")]
        public async Task<IActionResult> GetScreenById(int id)
        {
            var result = await _service.GetScreenByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateScreen")]
        public async Task<IActionResult> CreateScreen([FromBody] DentialClinic.Server.Models.WhatsApp.WhatsAppScreen request)
        {
            var result = await _service.CreateScreenAsync(request);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetScreenById), new { id = result.Result?.ScreenId }, result);
        }

        [HttpPut("UpdateScreen/{id}")]
        public async Task<IActionResult> UpdateScreen(int id, [FromBody] DentialClinic.Server.Models.WhatsApp.WhatsAppScreen request)
        {
            var result = await _service.UpdateScreenAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteScreen/{id}")]
        public async Task<IActionResult> DeleteScreen(int id)
        {
            var result = await _service.DeleteScreenAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Screen Options
        [HttpGet("GetOptionsByScreen/{screenId}")]
        public async Task<IActionResult> GetOptionsByScreen(int screenId)
        {
            var result = await _service.GetOptionsByScreenAsync(screenId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("CreateOption")]
        public async Task<IActionResult> CreateOption([FromBody] DentialClinic.Server.Models.WhatsApp.WhatsAppScreenOption request)
        {
            var result = await _service.CreateOptionAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("UpdateOption/{id}")]
        public async Task<IActionResult> UpdateOption(int id, [FromBody] DentialClinic.Server.Models.WhatsApp.WhatsAppScreenOption request)
        {
            var result = await _service.UpdateOptionAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteOption/{id}")]
        public async Task<IActionResult> DeleteOption(int id)
        {
            var result = await _service.DeleteOptionAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Conversation State
        [HttpGet("GetConversationState")]
        public async Task<IActionResult> GetConversationState([FromQuery] string fromNumber)
        {
            var result = await _service.GetConversationStateAsync(fromNumber);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("UpdateConversationState")]
        public async Task<IActionResult> UpdateConversationState([FromBody] DentialClinic.Server.Models.WhatsApp.WhatsAppConversationState request)
        {
            var result = await _service.UpdateConversationStateAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // Bookings
        [HttpGet("GetAllBookings")]
        public async Task<IActionResult> GetAllBookings([FromQuery] int? branchId)
        {
            var result = await _service.GetAllBookingsAsync(branchId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetBookingById/{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var result = await _service.GetBookingByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] DentialClinic.Server.Models.WhatsApp.WhatsAppBooking request)
        {
            var result = await _service.CreateBookingAsync(request);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetBookingById), new { id = result.Result?.WhatsAppBookingId }, result);
        }

        [HttpPut("UpdateBookingStatus/{id}")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromQuery] string status, [FromQuery] int? patientId)
        {
            var result = await _service.UpdateBookingStatusAsync(id, status, patientId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}

