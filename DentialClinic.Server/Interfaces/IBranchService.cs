using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Branch;

namespace DentialClinic.Server.Interfaces
{
    public interface IBranchService
    {
        Task<ServiceResponse<List<Branch>>> GetAllBranchesAsync();
        Task<ServiceResponse<Branch>> GetBranchByIdAsync(int id);
        Task<ServiceResponse<BranchDto>> CreateBranchAsync(BranchDto request);
        Task<ServiceResponse<BranchDto>> UpdateBranchAsync(int id, BranchDto request);
        Task<ServiceResponse<bool>> DeleteBranchAsync(int id);
        Task<ServiceResponse<List<BranchDto>>> GetActiveBranchesAsync();
    }
}

