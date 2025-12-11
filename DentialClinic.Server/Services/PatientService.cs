using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Patient;
using DentialClinic.Server.Models.Appointment;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;

        public PatientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Patient>>> GetAllPatientsAsync()
        {
            var response = new ServiceResponse<List<Patient>>();
            try
            {
                var patients = await _context.Patients.ToListAsync();
                response.Result = patients;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<Patient>> GetPatientByIdAsync(int id)
        {
            var response = new ServiceResponse<Patient>();
            try
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.PatientId == id);

                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found";
                    return response;
                }

                response.Result = patient;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<Patient>> CreatePatientAsync(PatientDto request)
        {
            var response = new ServiceResponse<Patient>();
            try
            {
                var patient = new Patient
                {
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Email = request.Email,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    Address = request.Address,
                    BranchId = request.BranchId,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                var getResponse = await GetPatientByIdAsync(patient.PatientId);
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

        public async Task<ServiceResponse<Patient>> UpdatePatientAsync(int id, PatientDto request)
        {
            var response = new ServiceResponse<Patient>();
            try
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found";
                    return response;
                }

                patient.FullName = request.FullName;
                patient.Phone = request.Phone;
                patient.Email = request.Email;
                patient.Gender = request.Gender;
                patient.DateOfBirth = request.DateOfBirth;
                patient.Address = request.Address;
                patient.BranchId = request.BranchId;
                patient.IsActive = request.IsActive;

                await _context.SaveChangesAsync();

                var getResponse = await GetPatientByIdAsync(id);
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

        public async Task<ServiceResponse<bool>> DeletePatientAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found";
                    return response;
                }

                // Check if patient has any appointments
                var hasAppointments = await _context.Appointments
                    .AnyAsync(a => a.PatientId == id);

                if (hasAppointments)
                {
                    response.Success = false;
                    response.Message = "Cannot delete patient. Patient has existing appointments. Please delete appointments first.";
                    return response;
                }

                _context.Patients.Remove(patient);
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

