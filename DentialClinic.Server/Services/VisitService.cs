using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Visit;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.Patient;
using DentialClinic.Server.Models.Doctor;
using DentialClinic.Server.Models.Appointment;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class VisitService : IVisitService
    {
        private readonly ApplicationDbContext _context;
        public VisitService(ApplicationDbContext context) => _context = context;

        private async Task<VisitDto> MapToDtoAsync(Visit v)
        {
            var treatments = await _context.VisitTreatments
                .Include(vt => vt.Treatment)
                .Where(vt => vt.VisitId == v.VisitId)
                .Select(vt => new VisitTreatmentDto
                {
                    VisitTreatmentId = vt.VisitTreatmentId,
                    TreatmentId = vt.TreatmentId,
                    TreatmentName = vt.Treatment.Name,
                    ToothNumber = vt.ToothNumber,
                    Quantity = vt.Quantity,
                    UnitPrice = vt.UnitPrice,
                    DiscountAmount = vt.DiscountAmount,
                    Notes = vt.Notes
                })
                .ToListAsync();

            return new VisitDto
            {
                VisitId = v.VisitId,
                BranchId = v.BranchId,
                BranchName = v.Branch?.Name ?? string.Empty,
                AppointmentId = v.AppointmentId,
                PatientId = v.PatientId,
                PatientName = v.Patient?.FullName ?? string.Empty,
                DoctorId = v.DoctorId,
                //DoctorName = v.Doctor?.User?.FullName ?? string.Empty,
                VisitTime = v.VisitTime,
                Diagnosis = v.Diagnosis,
                Notes = v.Notes,
                Status = v.Status,
                Treatments = treatments
            };
        }

        public async Task<ServiceResponse<List<VisitDto>>> GetAllVisitsAsync()
        {
            var response = new ServiceResponse<List<VisitDto>>();
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.Branch)
                    .Include(v => v.Patient)
                    .Include(v => v.Doctor).ThenInclude(d => d.UserId)
                    .OrderByDescending(v => v.VisitTime)
                    .ToListAsync();
                response.Result = new List<VisitDto>();
                foreach (var visit in visits)
                {
                    response.Result.Add(await MapToDtoAsync(visit));
                }
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<VisitDto>> GetVisitByIdAsync(int id)
        {
            var response = new ServiceResponse<VisitDto>();
            try
            {
                var visit = await _context.Visits
                    .Include(v => v.Branch)
                    .Include(v => v.Patient)
                    .Include(v => v.Doctor).ThenInclude(d => d.UserId)
                    .FirstOrDefaultAsync(v => v.VisitId == id);
                if (visit == null) { response.Success = false; response.Message = "Visit not found"; return response; }
                response.Result = await MapToDtoAsync(visit);
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<VisitDto>> CreateVisitAsync(VisitDto request)
        {
            var response = new ServiceResponse<VisitDto>();
            try
            {
                var visit = new Visit
                {
                    BranchId = request.BranchId,
                    AppointmentId = request.AppointmentId,
                    PatientId = request.PatientId,
                    DoctorId = request.DoctorId,
                    VisitTime = request.VisitTime,
                    Diagnosis = request.Diagnosis,
                    Notes = request.Notes,
                    Status = request.Status
                };
                _context.Visits.Add(visit);
                await _context.SaveChangesAsync();

                foreach (var treatment in request.Treatments)
                {
                    var vt = new VisitTreatment
                    {
                        VisitId = visit.VisitId,
                        TreatmentId = treatment.TreatmentId,
                        ToothNumber = treatment.ToothNumber,
                        Quantity = treatment.Quantity,
                        UnitPrice = treatment.UnitPrice,
                        DiscountAmount = treatment.DiscountAmount,
                        Notes = treatment.Notes
                    };
                    _context.VisitTreatments.Add(vt);
                }
                await _context.SaveChangesAsync();

                var getResponse = await GetVisitByIdAsync(visit.VisitId);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<VisitDto>> UpdateVisitAsync(int id, VisitDto request)
        {
            var response = new ServiceResponse<VisitDto>();
            try
            {
                var visit = await _context.Visits.FindAsync(id);
                if (visit == null) { response.Success = false; response.Message = "Visit not found"; return response; }
                visit.Diagnosis = request.Diagnosis;
                visit.Notes = request.Notes;
                visit.Status = request.Status;

                var existingTreatments = await _context.VisitTreatments.Where(vt => vt.VisitId == id).ToListAsync();
                _context.VisitTreatments.RemoveRange(existingTreatments);

                foreach (var treatment in request.Treatments)
                {
                    var vt = new VisitTreatment
                    {
                        VisitId = visit.VisitId,
                        TreatmentId = treatment.TreatmentId,
                        ToothNumber = treatment.ToothNumber,
                        Quantity = treatment.Quantity,
                        UnitPrice = treatment.UnitPrice,
                        DiscountAmount = treatment.DiscountAmount,
                        Notes = treatment.Notes
                    };
                    _context.VisitTreatments.Add(vt);
                }
                await _context.SaveChangesAsync();

                var getResponse = await GetVisitByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteVisitAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var visit = await _context.Visits.FindAsync(id);
                if (visit == null) { response.Success = false; response.Message = "Visit not found"; return response; }
                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<List<VisitDto>>> GetVisitsByBranchAsync(int branchId, DateTime? date)
        {
            var response = new ServiceResponse<List<VisitDto>>();
            try
            {
                var query = _context.Visits
                    .Include(v => v.Branch)
                    .Include(v => v.Patient)
                    .Include(v => v.Doctor).ThenInclude(d => d.UserId)
                    .Where(v => v.BranchId == branchId);
                if (date.HasValue) query = query.Where(v => v.VisitTime.Date == date.Value.Date);
                var visits = await query.OrderByDescending(v => v.VisitTime).ToListAsync();
                response.Result = new List<VisitDto>();
                foreach (var visit in visits)
                {
                    response.Result.Add(await MapToDtoAsync(visit));
                }
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<List<VisitDto>>> GetVisitsByPatientAsync(int patientId)
        {
            var response = new ServiceResponse<List<VisitDto>>();
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.Branch)
                    .Include(v => v.Patient)
                    .Include(v => v.Doctor).ThenInclude(d => d.UserId)
                    .Where(v => v.PatientId == patientId)
                    .OrderByDescending(v => v.VisitTime)
                    .ToListAsync();
                response.Result = new List<VisitDto>();
                foreach (var visit in visits)
                {
                    response.Result.Add(await MapToDtoAsync(visit));
                }
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<VisitDto>> CloseVisitAsync(int id)
        {
            var response = new ServiceResponse<VisitDto>();
            try
            {
                var visit = await _context.Visits.FindAsync(id);
                if (visit == null) { response.Success = false; response.Message = "Visit not found"; return response; }
                visit.Status = "CLOSED";
                await _context.SaveChangesAsync();
                var getResponse = await GetVisitByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }
    }
}

