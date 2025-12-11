using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Doctor;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;

        public DoctorService(ApplicationDbContext context) => _context = context;

        public async Task<ServiceResponse<List<VDoctorDetails>>> GetViewDoctorsAsync()
        {
            var response = new ServiceResponse<List<VDoctorDetails>>();
            try
            {
                var doctors = await _context.VDoctorDetails
                    .OrderBy(d => d.DoctorName)
                    .ToListAsync();

                response.Result = doctors;
                response.Success = true;
                response.Message = "Doctors retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving doctors: {ex.Message}";
            }
            return response;
        }

        //public async Task<ServiceResponse<List<DoctorDto>>> GetAllDoctorsAsync()
        //{
        //    var response = new ServiceResponse<List<DoctorDto>>();
        //    try
        //    {
        //        response.Result = _context.Doctors.ToListAsync();
        //        response.Success = true;
        //    }
        //    catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
        //    return response;
        //}

        //public async Task<ServiceResponse<DoctorDto>> GetDoctorByIdAsync(int id)
        //{
        //    var response = new ServiceResponse<DoctorDto>();
        //    try
        //    {
        //        var doctor = await _context.Doctors
        //            .Include(d => d.User)
        //            .Include(d => d.Branch)
        //            .Where(d => d.DoctorId == id)
        //            .Select(d => new DoctorDto
        //            {
        //                DoctorId = d.DoctorId,
        //                UserId = d.UserId,
        //                UserName = d.User.FirstName,
        //                UserEmail = d.User.Email,
        //                BranchId = d.BranchId,
        //                BranchName = d.Branch != null ? d.Branch.Name : null,
        //                Speciality = d.Speciality,
        //                DefaultFee = d.DefaultFee,
        //                IsActive = d.IsActive
        //            })
        //            .FirstOrDefaultAsync();
        //        if (doctor == null) { response.Success = false; response.Message = "Doctor not found"; return response; }
        //        response.Result = doctor;
        //        response.Success = true;
        //    }
        //    catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
        //    return response;
        //}

        //public async Task<ServiceResponse<DoctorDto>> GetDoctorByUserIdAsync(int userId)
        //{
        //    var response = new ServiceResponse<DoctorDto>();
        //    try
        //    {
        //        var doctor = await _context.Doctors
        //            .Include(d => d.User)
        //            .Include(d => d.Branch)
        //            .Where(d => d.UserId == userId)
        //            .Select(d => new DoctorDto
        //            {
        //                DoctorId = d.DoctorId,
        //                UserId = d.UserId,
        //                UserName = d.User.FirstName,
        //                UserEmail = d.User.Email,
        //                BranchId = d.BranchId,
        //                BranchName = d.Branch != null ? d.Branch.Name : null,
        //                Speciality = d.Speciality,
        //                DefaultFee = d.DefaultFee,
        //                IsActive = d.IsActive
        //            })
        //            .FirstOrDefaultAsync();
        //        if (doctor == null) { response.Success = false; response.Message = "Doctor not found"; return response; }
        //        response.Result = doctor;
        //        response.Success = true;
        //    }
        //    catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
        //    return response;
        //}

        public async Task<ServiceResponse<DoctorDto>> CreateDoctorAsync(DoctorDto request)
        {
            var response = new ServiceResponse<DoctorDto>();

            try
            {
                // 1) Validate: user must not already be a doctor
                var doctorExistsForUser = await _context.Doctors
                    .AnyAsync(d => d.UserId == request.UserId);

                if (doctorExistsForUser)
                {
                    response.Success = false;
                    response.Message = "This user is already assigned as a doctor.";
                    return response;
                }

                // 2) Create doctor
                var doctor = new Doctor
                {
                    UserId = request.UserId,
                    BranchId = request.BranchId,
                    Speciality = request.Speciality,
                    DefaultFee = request.DefaultFee,
                    IsActive = request.IsActive
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                // 3) Optionally map back to DTO (if you have a mapper)
                response.Success = true;
                response.Message = "Doctor created successfully";

                // If you want to return created data:
                // response.Result = new DoctorDto
                // {
                //     DoctorId = doctor.DoctorId,
                //     UserId = doctor.UserId,
                //     BranchId = doctor.BranchId,
                //     Speciality = doctor.Speciality,
                //     DefaultFee = doctor.DefaultFee,
                //     IsActive = doctor.IsActive
                // };

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error creating doctor: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<DoctorDto>> UpdateDoctorAsync(int id, DoctorDto request)
        {
            var response = new ServiceResponse<DoctorDto>();

            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    response.Success = false;
                    response.Message = "Doctor not found.";
                    return response;
                }

                // If UserId is allowed to change, enforce uniqueness
                if (request.UserId != default && request.UserId != doctor.UserId)
                {
                    var doctorExistsForUser = await _context.Doctors
                        .AnyAsync(d => d.UserId == request.UserId && d.DoctorId != id);

                    if (doctorExistsForUser)
                    {
                        response.Success = false;
                        response.Message = "This user is already assigned as a doctor.";
                        return response;
                    }

                    doctor.UserId = request.UserId;
                }

                doctor.BranchId = request.BranchId;
                doctor.Speciality = request.Speciality;
                doctor.DefaultFee = request.DefaultFee;
                doctor.IsActive = request.IsActive;

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Doctor updated successfully";

                // If you want to return updated data:
                // response.Result = new DoctorDto
                // {
                //     DoctorId = doctor.DoctorId,
                //     UserId = doctor.UserId,
                //     BranchId = doctor.BranchId,
                //     Speciality = doctor.Speciality,
                //     DefaultFee = doctor.DefaultFee,
                //     IsActive = doctor.IsActive
                // };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating doctor: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteDoctorAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null) { response.Success = false; response.Message = "Doctor not found"; return response; }
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        //public async Task<ServiceResponse<List<DoctorDto>>> GetDoctorsByBranchAsync(int branchId)
        //{
        //    var response = new ServiceResponse<List<DoctorDto>>();
        //    try
        //    {
        //        response.Result = await _context.Doctors
        //            .Include(d => d.User)
        //            .Include(d => d.Branch)
        //            .Where(d => d.BranchId == branchId)
        //            .Select(d => new DoctorDto
        //            {
        //                DoctorId = d.DoctorId,
        //                UserId = d.UserId,
        //                UserName = d.User.FirstName,
        //                UserEmail = d.User.Email,
        //                BranchId = d.BranchId,
        //                BranchName = d.Branch != null ? d.Branch.Name : null,
        //                Speciality = d.Speciality,
        //                DefaultFee = d.DefaultFee,
        //                IsActive = d.IsActive
        //            })
        //            .ToListAsync();
        //        response.Success = true;
        //    }
        //    catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
        //    return response;
        //}

        //public async Task<ServiceResponse<List<DoctorDto>>> GetActiveDoctorsAsync()
        //{
        //    var response = new ServiceResponse<List<DoctorDto>>();
        //    try
        //    {
        //        response.Result = await _context.Doctors
        //            .Include(d => d.User)
        //            .Include(d => d.Branch)
        //            .Where(d => d.IsActive)
        //            .Select(d => new DoctorDto
        //            {
        //                DoctorId = d.DoctorId,
        //                UserId = d.UserId,
        //                UserName = d.User.FirstName,
        //                UserEmail = d.User.Email,
        //                BranchId = d.BranchId,
        //                BranchName = d.Branch != null ? d.Branch.Name : null,
        //                Speciality = d.Speciality,
        //                DefaultFee = d.DefaultFee,
        //                IsActive = d.IsActive
        //            })
        //            .ToListAsync();
        //        response.Success = true;
        //    }
        //    catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
        //    return response;
        //}
    }
}

