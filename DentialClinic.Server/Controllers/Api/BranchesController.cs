using Microsoft.AspNetCore.Mvc;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models.Branch;

namespace DentialClinic.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _service;

        public BranchesController(IBranchService service)
        {
            _service = service;
        }

        [HttpGet("GetAllBranches")]
        public async Task<IActionResult> GetAllBranches()
        {
            var result = await _service.GetAllBranchesAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetBranchById/{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var result = await _service.GetBranchByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("CreateBranch")]
        public async Task<IActionResult> CreateBranch([FromBody] BranchDto request)
        {
            var result = await _service.CreateBranchAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("UpdateBranch/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] BranchDto request)
        {
            var result = await _service.UpdateBranchAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("DeleteBranch/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _service.DeleteBranchAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("GetActiveBranches")]
        public async Task<IActionResult> GetActiveBranches()
        {
            var result = await _service.GetActiveBranchesAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}

