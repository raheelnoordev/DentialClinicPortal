using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Branch;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _context;

        public BranchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Branch>>> GetAllBranchesAsync()
        {
            var response = new ServiceResponse<List<Branch>>();

            try
            {
                var branches = await _context.Branches
                    .OrderBy(b => b.Name)
                    .Select(b => new Branch
                    {
                        BranchId = b.BranchId,
                        Name = b.Name,
                        Code = b.Code,
                        Phone = b.Phone,
                        WhatsAppNumber = b.WhatsAppNumber,
                        Email = b.Email,
                        Address = b.Address,
                        City = b.City,
                        IsActive = b.IsActive,
                        CreatedAt = b.CreatedAt
                    })
                    .ToListAsync();

                response.Result = branches;
                response.Success = true;
                response.Message = "Branches retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving branches: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<Branch>> GetBranchByIdAsync(int id)
        {
            var response = new ServiceResponse<Branch>();

            try
            {
                var branch = await _context.Branches
                    .Where(b => b.BranchId == id)
                    .Select(b => new Branch
                    {
                        BranchId = b.BranchId,
                        Name = b.Name,
                        Code = b.Code,
                        Phone = b.Phone,
                        WhatsAppNumber = b.WhatsAppNumber,
                        Email = b.Email,
                        Address = b.Address,
                        City = b.City,
                        IsActive = b.IsActive,
                        CreatedAt = b.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (branch == null)
                {
                    response.Success = false;
                    response.Message = "Branch not found";
                    return response;
                }

                response.Result = branch;
                response.Success = true;
                response.Message = "Branch retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving branch: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<BranchDto>> CreateBranchAsync(BranchDto request)
        {
            var response = new ServiceResponse<BranchDto>();

            try
            {
                if (!string.IsNullOrWhiteSpace(request.Code))
                {
                    var codeExists = await _context.Branches
                        .AnyAsync(b => b.Code == request.Code);

                    if (codeExists)
                    {
                        response.Success = false;
                        response.Message = "Branch code already exists. Please use a different code.";
                        return response;
                    }
                }

                var branch = new Branch
                {
                    Name = request.Name,
                    Code = request.Code,
                    Phone = request.Phone,
                    WhatsAppNumber = request.WhatsAppNumber,
                    Email = request.Email,
                    Address = request.Address,
                    City = request.City,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Branch created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error creating branch: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<BranchDto>> UpdateBranchAsync(int id, BranchDto request)
        {
            var response = new ServiceResponse<BranchDto>();

            try
            {
                var branch = await _context.Branches.FindAsync(id);

                if (branch == null)
                {
                    response.Success = false;
                    response.Message = "Branch not found";
                    return response;
                }

                branch.Name = request.Name;
                branch.Code = request.Code;
                branch.Phone = request.Phone;
                branch.WhatsAppNumber = request.WhatsAppNumber;
                branch.Email = request.Email;
                branch.Address = request.Address;
                branch.City = request.City;
                branch.IsActive = request.IsActive;

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Branch updated successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating branch: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteBranchAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var branch = await _context.Branches.FindAsync(id);

                if (branch == null)
                {
                    response.Success = false;
                    response.Message = "Branch not found";
                    return response;
                }

                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();

                response.Result = true;
                response.Success = true;
                response.Message = "Branch deleted successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting branch: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<BranchDto>>> GetActiveBranchesAsync()
        {
            var response = new ServiceResponse<List<BranchDto>>();

            try
            {
                var branches = await _context.Branches
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.Name)
                    .Select(b => new Branch
                    {
                        //BranchId = b.BranchId,
                        Name = b.Name,
                        Code = b.Code,
                        Phone = b.Phone,
                        WhatsAppNumber = b.WhatsAppNumber,
                        Email = b.Email,
                        Address = b.Address,
                        City = b.City,
                        IsActive = b.IsActive,
                        CreatedAt = b.CreatedAt
                    })
                    .ToListAsync();

                //response.Result = branches;
                response.Success = true;
                response.Message = "Active branches retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving active branches: {ex.Message}";
            }

            return response;
        }
    }
}

