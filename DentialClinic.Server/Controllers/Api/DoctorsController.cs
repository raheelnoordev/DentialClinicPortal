using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models.Doctor;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _service;

        public DoctorsController(IDoctorService service)
        {
            _service = service;
        }

        [HttpGet("ViewDoctors")]
        public async Task<IActionResult> ViewDoctors()
        {
            var result = await _service.GetViewDoctorsAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        //[HttpGet("GetDoctorById/{id}")]
        //public async Task<IActionResult> GetDoctorById(int id)
        //{
        //    var result = await _service.GetDoctorByIdAsync(id);
        //    if (!result.Success) return NotFound(result);
        //    return Ok(result);
        //}

        //[HttpGet("GetDoctorByUserId/{userId}")]
        //public async Task<IActionResult> GetDoctorByUserId(int userId)
        //{
        //    var result = await _service.GetDoctorByUserIdAsync(userId);
        //    if (!result.Success) return NotFound(result);
        //    return Ok(result);
        //}

        [HttpPost("CreateDoctor")]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorDto request)
        {
            var result = await _service.CreateDoctorAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
            //return CreatedAtAction(nameof(GetDoctorById), new { id = result.Result?.DoctorId }, result);
        }

        [HttpPut("UpdateDoctor/{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorDto request)
        {
            var result = await _service.UpdateDoctorAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteDoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _service.DeleteDoctorAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        //[HttpGet("GetDoctorsByBranch/{branchId}")]
        //public async Task<IActionResult> GetDoctorsByBranch(int branchId)
        //{
        //    var result = await _service.GetDoctorsByBranchAsync(branchId);
        //    if (!result.Success) return BadRequest(result);
        //    return Ok(result);
        //}

        //[HttpGet("GetActiveDoctors")]
        //public async Task<IActionResult> GetActiveDoctors()
        //{
        //    var result = await _service.GetActiveDoctorsAsync();
        //    if (!result.Success) return BadRequest(result);
        //    return Ok(result);
        //}
    }
}

