using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Patient;

namespace DentialClinic.Server.Interfaces
{
    public interface IPatientService
    {
        Task<ServiceResponse<List<Patient>>> GetAllPatientsAsync();
        Task<ServiceResponse<Patient>> GetPatientByIdAsync(int id);
        Task<ServiceResponse<Patient>> CreatePatientAsync(PatientDto request);
        Task<ServiceResponse<Patient>> UpdatePatientAsync(int id, PatientDto request);
        Task<ServiceResponse<bool>> DeletePatientAsync(int id);
    }
}

