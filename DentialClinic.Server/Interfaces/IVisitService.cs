using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Visit;

namespace DentialClinic.Server.Interfaces
{
    public interface IVisitService
    {
        Task<ServiceResponse<List<VisitDto>>> GetAllVisitsAsync();
        Task<ServiceResponse<VisitDto>> GetVisitByIdAsync(int id);
        Task<ServiceResponse<VisitDto>> CreateVisitAsync(VisitDto request);
        Task<ServiceResponse<VisitDto>> UpdateVisitAsync(int id, VisitDto request);
        Task<ServiceResponse<bool>> DeleteVisitAsync(int id);
        Task<ServiceResponse<List<VisitDto>>> GetVisitsByBranchAsync(int branchId, DateTime? date);
        Task<ServiceResponse<List<VisitDto>>> GetVisitsByPatientAsync(int patientId);
        Task<ServiceResponse<VisitDto>> CloseVisitAsync(int id);
    }
}

