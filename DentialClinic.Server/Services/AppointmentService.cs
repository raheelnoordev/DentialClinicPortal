using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Appointment;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        private AppointmentDto MapToDto(Appointment a, Dictionary<int, string>? doctorNames = null)
        {
            string doctorName = string.Empty;
            if (a.Doctor != null && a.Doctor.UserId > 0)
            {
                if (doctorNames != null && doctorNames.ContainsKey(a.Doctor.UserId))
                {
                    doctorName = doctorNames[a.Doctor.UserId];
                }
                else
                {
                    var user = _context.Users.FirstOrDefault(u => u.UserId == a.Doctor.UserId);
                    doctorName = user?.FullName ?? string.Empty;
                }
            }

            return new AppointmentDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                PatientName = a.Patient?.FullName ?? string.Empty,
                PatientPhone = a.Patient?.Phone,
                DoctorId = a.DoctorId,
                DoctorName = doctorName,
                BranchId = a.BranchId,
                BranchName = a.Branch?.Name ?? string.Empty,
                AppointmentAt = a.AppointmentAt,
                DurationMin = a.DurationMin,
                Status = a.Status,
                Source = a.Source,
                Reason = a.Reason,
                CreatedByUserId = a.CreatedByUserId,
                CreatedByUserName = a.CreatedByUser?.FullName,
                CreatedAt = a.CreatedAt
            };
        }

        public async Task<ServiceResponse<List<AppointmentDto>>> GetAllAppointmentsAsync(int? branchId = null, int? doctorId = null, string? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var response = new ServiceResponse<List<AppointmentDto>>();
            try
            {
                var query = _context.Appointments
                    .Include(a => a.Branch)
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Include(a => a.CreatedByUser)
                    .AsQueryable();

                if (branchId.HasValue)
                    query = query.Where(a => a.BranchId == branchId.Value);

                if (doctorId.HasValue)
                    query = query.Where(a => a.DoctorId == doctorId.Value);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(a => a.Status == status);

                if (dateFrom.HasValue)
                    query = query.Where(a => a.AppointmentAt >= dateFrom.Value);

                if (dateTo.HasValue)
                    query = query.Where(a => a.AppointmentAt <= dateTo.Value);

                var appointments = await query
                    .OrderByDescending(a => a.AppointmentAt)
                    .ToListAsync();

                // Load doctor names efficiently
                var doctorUserIds = appointments
                    .Where(a => a.Doctor != null && a.Doctor.UserId > 0)
                    .Select(a => a.Doctor.UserId)
                    .Distinct()
                    .ToList();

                var doctorNames = await _context.Users
                    .Where(u => doctorUserIds.Contains(u.UserId))
                    .ToDictionaryAsync(u => u.UserId, u => u.FullName);

                response.Result = appointments.Select(a => MapToDto(a, doctorNames)).ToList();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AppointmentDto>> GetAppointmentByIdAsync(int id)
        {
            var response = new ServiceResponse<AppointmentDto>();
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Branch)
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Include(a => a.CreatedByUser)
                    .FirstOrDefaultAsync(a => a.AppointmentId == id);

                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found";
                    return response;
                }

                // Load doctor name
                var doctorName = string.Empty;
                if (appointment.Doctor != null && appointment.Doctor.UserId > 0)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == appointment.Doctor.UserId);
                    doctorName = user?.FullName ?? string.Empty;
                }

                var doctorNames = appointment.Doctor != null && appointment.Doctor.UserId > 0
                    ? new Dictionary<int, string> { { appointment.Doctor.UserId, doctorName } }
                    : null;

                response.Result = MapToDto(appointment, doctorNames);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private bool HasOverlappingAppointment(int doctorId, DateTime appointmentAt, int durationMin, int? excludeAppointmentId = null)
        {
            var appointmentEnd = appointmentAt.AddMinutes(durationMin);

            var query = _context.Appointments
                .Where(a => a.DoctorId == doctorId
                    && a.Status != "cancelled"
                    && a.Status != "completed"
                    && a.Status != "no_show"
                    && ((a.AppointmentAt <= appointmentAt && a.AppointmentAt.AddMinutes(a.DurationMin) > appointmentAt)
                        || (a.AppointmentAt < appointmentEnd && a.AppointmentAt.AddMinutes(a.DurationMin) >= appointmentEnd)
                        || (a.AppointmentAt >= appointmentAt && a.AppointmentAt.AddMinutes(a.DurationMin) <= appointmentEnd)));

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.AppointmentId != excludeAppointmentId.Value);

            return query.Any();
        }

        public async Task<ServiceResponse<AppointmentDto>> CreateAppointmentAsync(AppointmentDto request)
        {
            var response = new ServiceResponse<AppointmentDto>();
            try
            {
                // Convert DateTime to Unspecified kind for PostgreSQL timestamp without time zone
                var appointmentAt = request.AppointmentAt;
                if (appointmentAt.Kind == DateTimeKind.Utc)
                {
                    appointmentAt = DateTime.SpecifyKind(appointmentAt, DateTimeKind.Unspecified);
                }
                else if (appointmentAt.Kind == DateTimeKind.Local)
                {
                    appointmentAt = appointmentAt.ToUniversalTime();
                    appointmentAt = DateTime.SpecifyKind(appointmentAt, DateTimeKind.Unspecified);
                }

                // Check for overlapping appointments
                if (HasOverlappingAppointment(request.DoctorId, appointmentAt, request.DurationMin))
                {
                    response.Success = false;
                    response.Message = "Doctor already has an appointment at this time. Please choose a different time.";
                    return response;
                }

                var appointment = new Appointment
                {
                    PatientId = request.PatientId,
                    DoctorId = request.DoctorId,
                    BranchId = request.BranchId,
                    AppointmentAt = appointmentAt,
                    DurationMin = request.DurationMin,
                    Status = request.Status,
                    Source = request.Source,
                    Reason = request.Reason,
                    CreatedByUserId = request.CreatedByUserId,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                var getResponse = await GetAppointmentByIdAsync(appointment.AppointmentId);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AppointmentDto>> UpdateAppointmentAsync(int id, AppointmentDto request)
        {
            var response = new ServiceResponse<AppointmentDto>();
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found";
                    return response;
                }

                // Convert DateTime to Unspecified kind for PostgreSQL timestamp without time zone
                var appointmentAt = request.AppointmentAt;
                if (appointmentAt.Kind == DateTimeKind.Utc)
                {
                    appointmentAt = DateTime.SpecifyKind(appointmentAt, DateTimeKind.Unspecified);
                }
                else if (appointmentAt.Kind == DateTimeKind.Local)
                {
                    appointmentAt = appointmentAt.ToUniversalTime();
                    appointmentAt = DateTime.SpecifyKind(appointmentAt, DateTimeKind.Unspecified);
                }

                // Check for overlapping appointments (excluding current appointment)
                if (HasOverlappingAppointment(request.DoctorId, appointmentAt, request.DurationMin, id))
                {
                    response.Success = false;
                    response.Message = "Doctor already has an appointment at this time. Please choose a different time.";
                    return response;
                }

                appointment.PatientId = request.PatientId;
                appointment.DoctorId = request.DoctorId;
                appointment.BranchId = request.BranchId;
                appointment.AppointmentAt = appointmentAt;
                appointment.DurationMin = request.DurationMin;
                appointment.Status = request.Status;
                appointment.Source = request.Source;
                appointment.Reason = request.Reason;

                await _context.SaveChangesAsync();

                var getResponse = await GetAppointmentByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AppointmentDto>> UpdateAppointmentStatusAsync(int id, string status)
        {
            var response = new ServiceResponse<AppointmentDto>();
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found";
                    return response;
                }

                appointment.Status = status;
                await _context.SaveChangesAsync();

                var getResponse = await GetAppointmentByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAppointmentAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Appointment not found";
                    return response;
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
