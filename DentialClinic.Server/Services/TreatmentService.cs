using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Treatment;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly ApplicationDbContext _context;
        public TreatmentService(ApplicationDbContext context) => _context = context;

        public async Task<ServiceResponse<List<TreatmentCatalogDto>>> GetAllTreatmentsAsync()
        {
            var response = new ServiceResponse<List<TreatmentCatalogDto>>();
            try
            {
                response.Result = await _context.TreatmentCatalogs
                    .Select(t => new TreatmentCatalogDto
                    {
                        TreatmentId = t.TreatmentId,
                        Name = t.Name,
                        Description = t.Description,
                        DefaultPrice = t.DefaultPrice,
                        DefaultDurationMinutes = t.DefaultDurationMinutes,
                        IsActive = t.IsActive
                    })
                    .OrderBy(t => t.Name)
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<TreatmentCatalogDto>> GetTreatmentByIdAsync(int id)
        {
            var response = new ServiceResponse<TreatmentCatalogDto>();
            try
            {
                var treatment = await _context.TreatmentCatalogs
                    .Where(t => t.TreatmentId == id)
                    .Select(t => new TreatmentCatalogDto
                    {
                        TreatmentId = t.TreatmentId,
                        Name = t.Name,
                        Description = t.Description,
                        DefaultPrice = t.DefaultPrice,
                        DefaultDurationMinutes = t.DefaultDurationMinutes,
                        IsActive = t.IsActive
                    })
                    .FirstOrDefaultAsync();
                if (treatment == null) { response.Success = false; response.Message = "Treatment not found"; return response; }
                response.Result = treatment;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<TreatmentCatalogDto>> CreateTreatmentAsync(TreatmentCatalogDto request)
        {
            var response = new ServiceResponse<TreatmentCatalogDto>();
            try
            {
                var treatment = new TreatmentCatalog
                {
                    Name = request.Name,
                    Description = request.Description,
                    DefaultPrice = request.DefaultPrice,
                    DefaultDurationMinutes = request.DefaultDurationMinutes,
                    IsActive = request.IsActive
                };
                _context.TreatmentCatalogs.Add(treatment);
                await _context.SaveChangesAsync();
                var getResponse = await GetTreatmentByIdAsync(treatment.TreatmentId);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<TreatmentCatalogDto>> UpdateTreatmentAsync(int id, TreatmentCatalogDto request)
        {
            var response = new ServiceResponse<TreatmentCatalogDto>();
            try
            {
                var treatment = await _context.TreatmentCatalogs.FindAsync(id);
                if (treatment == null) { response.Success = false; response.Message = "Treatment not found"; return response; }
                treatment.Name = request.Name;
                treatment.Description = request.Description;
                treatment.DefaultPrice = request.DefaultPrice;
                treatment.DefaultDurationMinutes = request.DefaultDurationMinutes;
                treatment.IsActive = request.IsActive;
                await _context.SaveChangesAsync();
                var getResponse = await GetTreatmentByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteTreatmentAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var treatment = await _context.TreatmentCatalogs.FindAsync(id);
                if (treatment == null) { response.Success = false; response.Message = "Treatment not found"; return response; }
                _context.TreatmentCatalogs.Remove(treatment);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<List<TreatmentCatalogDto>>> GetActiveTreatmentsAsync()
        {
            var response = new ServiceResponse<List<TreatmentCatalogDto>>();
            try
            {
                response.Result = await _context.TreatmentCatalogs
                    .Where(t => t.IsActive)
                    .Select(t => new TreatmentCatalogDto
                    {
                        TreatmentId = t.TreatmentId,
                        Name = t.Name,
                        Description = t.Description,
                        DefaultPrice = t.DefaultPrice,
                        DefaultDurationMinutes = t.DefaultDurationMinutes,
                        IsActive = t.IsActive
                    })
                    .OrderBy(t => t.Name)
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }
    }
}

