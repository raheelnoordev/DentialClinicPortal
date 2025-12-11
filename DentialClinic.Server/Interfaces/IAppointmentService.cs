using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Appointment;

namespace DentialClinic.Server.Interfaces
{
    public interface IAppointmentService
    {
        Task<ServiceResponse<List<AppointmentDto>>> GetAllAppointmentsAsync(int? branchId = null, int? doctorId = null, string? status = null, DateTime? dateFrom = null, DateTime? dateTo = null);
        Task<ServiceResponse<AppointmentDto>> GetAppointmentByIdAsync(int id);
        Task<ServiceResponse<AppointmentDto>> CreateAppointmentAsync(AppointmentDto request);
        Task<ServiceResponse<AppointmentDto>> UpdateAppointmentAsync(int id, AppointmentDto request);
        Task<ServiceResponse<AppointmentDto>> UpdateAppointmentStatusAsync(int id, string status);
        Task<ServiceResponse<bool>> DeleteAppointmentAsync(int id);
    }
}

