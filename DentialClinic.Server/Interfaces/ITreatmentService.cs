using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Treatment;

namespace DentialClinic.Server.Interfaces
{
    public interface ITreatmentService
    {
        Task<ServiceResponse<List<TreatmentCatalogDto>>> GetAllTreatmentsAsync();
        Task<ServiceResponse<TreatmentCatalogDto>> GetTreatmentByIdAsync(int id);
        Task<ServiceResponse<TreatmentCatalogDto>> CreateTreatmentAsync(TreatmentCatalogDto request);
        Task<ServiceResponse<TreatmentCatalogDto>> UpdateTreatmentAsync(int id, TreatmentCatalogDto request);
        Task<ServiceResponse<bool>> DeleteTreatmentAsync(int id);
        Task<ServiceResponse<List<TreatmentCatalogDto>>> GetActiveTreatmentsAsync();
    }
}

