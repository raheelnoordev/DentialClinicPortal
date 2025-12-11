using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Doctor;

namespace DentialClinic.Server.Interfaces
{
    public interface IDoctorService
    {
        Task<ServiceResponse<List<VDoctorDetails>>> GetViewDoctorsAsync();
        Task<ServiceResponse<DoctorDto>> CreateDoctorAsync(DoctorDto request);
        Task<ServiceResponse<DoctorDto>> UpdateDoctorAsync(int id, DoctorDto request);
        Task<ServiceResponse<bool>> DeleteDoctorAsync(int id);
    }
}

