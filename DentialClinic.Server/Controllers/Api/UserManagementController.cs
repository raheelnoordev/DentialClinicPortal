using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Company;
using DentialClinic.Server.Models.UserManagement;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    { 
        private IUserManagement _interfaceUserManagement;
        private readonly ApplicationDbContext _context;

        public UserManagementController(IUserManagement interfaceUserManagement, ApplicationDbContext context)
        {
            _interfaceUserManagement = interfaceUserManagement;
            _context = context;
        }

        #region USER
        //[Authorize]
        //[HttpGet("GetUsersList")]

        //public ServiceResponse<List<Users>> GetUsersList()
        //{
        //    var data = _interfaceUserManagement.GetUsersList();
        //    return data;
        //}

        //[Authorize]

        [HttpGet("GetUsersList")]
        public ServiceResponse<List<Users>> GetUsersList()
        {
            var response = new ServiceResponse<List<Users>>();
            try
            {
                var users = _context.Users.ToList();

                response.Result = users;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

      
        [HttpGet("GetUserById")]
        public ServiceResponse<Users> GetUserById(int userId)
        {
            var data = _interfaceUserManagement.GetUserById(userId);
            return data;
        }

        //[Authorize]
        //[HttpPost("SaveUser")]
        //public ServiceResponse<Users> SaveUser(Users user)
        //{
        //    var data = _interfaceUserManagement.SaveUser(user);
        //    return data;
        //}

        //[Authorize]

        [HttpPost("SaveUser")]
        public async Task<ServiceResponse<Users>> SaveUser([FromBody] SaveUserRequest request)
        {
            var response = new ServiceResponse<Users>();
            try
            {
                // Check if email already exists
                var emailExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == request.Email);
                if (emailExists)
                {
                    response.Success = false;
                    response.Message = "Email already exists. Please use a different email.";
                    return response;
                }

                // Create user object
                var user = new Users
                {
                    BranchId = request.BranchId,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    PasswordHash = request.PasswordHash,
                    RoleId = request.RoleId,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                // Save user directly
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                response.Success = true;
                response.Message = "User saved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPut("UpdateUser")]
        public async Task<ServiceResponse<Users>> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var response = new ServiceResponse<Users>();

            try
            {
                // 1) Check if email already exists for another user
                var emailExists = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == request.Email && u.UserId != request.UserId);

                if (emailExists)
                {
                    response.Success = false;
                    response.Message = "Email already exists. Please use a different email.";
                    return response;
                }

                // 2) Get existing user AS TRACKED (no AsNoTracking here!)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == request.UserId);

                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                // 3) Update properties
                existingUser.BranchId = request.BranchId;
                existingUser.FullName = request.FullName;
                existingUser.Email = request.Email;
                existingUser.Phone = request.Phone;
                existingUser.IsActive = request.IsActive;
                existingUser.RoleId = request.RoleId;

                // Update password only if provided
                if (!string.IsNullOrEmpty(request.PasswordHash))
                {
                    existingUser.PasswordHash = request.PasswordHash;
                }

                // 4) Save changes
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "User updated successfully";
                response.Result = existingUser;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        //[Authorize]
        [HttpDelete("DeleteUserById")]
        public ServiceResponse<bool> DeleteUserById(int userId)
        {
            var data = _interfaceUserManagement.DeleteUserById(userId);
            return data;
        }

        //[Authorize]
        [HttpGet("DeactivateUserById")]
        public ServiceResponse<bool> DeactivateUserById(int userId)
        {
            var data = _interfaceUserManagement.DeactivateUserById(userId);
            return data;
        }

        //[Authorize]
        [HttpGet("ChangePassword")]
        public ServiceResponse<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var data = _interfaceUserManagement.ChangePassword(userId, oldPassword, newPassword);
            return data;
        }

        //[Authorize]
        [HttpGet("ResetPassword")]
        public ServiceResponse<bool> ResetPassword(int userId, string newPassword)
        {
            var data = _interfaceUserManagement.ResetPassword(userId, newPassword);
            return data;
        }
        #endregion

        #region ROLE
        //[Authorize]
        [HttpGet("GetRoleList")]
        public ServiceResponse<List<Roles>> GetRoleList()
        {
            var data = _interfaceUserManagement.GetRoleList();
            return data;
        }

        //[Authorize]
        [HttpGet("GetRoleById")]
        public ServiceResponse<Roles> GetRoleById(int roleId)
        {
            var data = _interfaceUserManagement.GetRoleById(roleId);
            return data;
        }

        //[Authorize]
        [HttpPost("SaveRole")]
        public ServiceResponse<Roles> SaveRole(Roles role)
        {
            var data = _interfaceUserManagement.SaveRole(role);
            return data;
        }

        //[Authorize]
        [HttpPut("UpdateRole")]
        public ServiceResponse<Roles> UpdateRole(Roles role)
        {
            var data = _interfaceUserManagement.UpdateRole(role);
            return data;
        }

        //[Authorize]
        [HttpDelete("DeleteRoleById")]
        public ServiceResponse<bool> DeleteRoleById(int roleId)
        {
            var data = _interfaceUserManagement.DeleteRoleById(roleId);
            return data;
        }
        #endregion

        #region MENU
        //[Authorize]
        [HttpGet("GetMenuList")]
        public ServiceResponse<List<Menus>> GetMenuList()
        {
            var data = _interfaceUserManagement.GetMenuList();
            return data;
        }

        //[Authorize]
        [HttpGet("GetMenuById")]
        public ServiceResponse<Menus> GetMenuById(int menuId)
        {
            var data = _interfaceUserManagement.GetMenuById(menuId);
            return data;
        }

        //[Authorize]
        [HttpPost("SaveMenu")]
        public ServiceResponse<Menus> SaveMenu(Menus menu)
        {
            var data = _interfaceUserManagement.SaveMenu(menu);
            return data;
        }

        //[Authorize]
        [HttpPut("UpdateMenu")]
        public ServiceResponse<Menus> UpdateMenu(Menus menu)
        {
            var data = _interfaceUserManagement.UpdateMenu(menu);
            return data;
        }

        //[Authorize]
        [HttpDelete("DeleteMenuById")]
        public ServiceResponse<bool> DeleteMenuById(int menuId)
        {
            var data = _interfaceUserManagement.DeleteMenuById(menuId);
            return data;
        }
        #endregion


        #region USER ROLE
        //[Authorize]
        [HttpGet("GetUserRolesByUserId")]
        public ServiceResponse<List<UserRole>> GetUserRolesByUserId(int userId)
        {
            var data = _interfaceUserManagement.GetUserRolesByUserId(userId);
            return data;
        }

        //[Authorize]
        [HttpPost("AssignUserRole")]
        public ServiceResponse<UserRole> AssignUserRole(UserRole userRole)
        {
            var data = _interfaceUserManagement.AssignUserRole(userRole);
            return data;
        }

        //[Authorize]
        [HttpDelete("DeleteUserRoleById")]
        public ServiceResponse<bool> DeleteUserRoleById(int userId, int roleId)
        {
            var data = _interfaceUserManagement.DeleteUserRoleById(userId, roleId);
            return data;
        }
        #endregion

        #region MENU ROLE
        //[Authorize]
        [HttpGet("GetMenuRolesByRoleId")]
        public ServiceResponse<List<RoleMenus>> GetMenuRolesByRoleId(int roleId)
        {
            var data = _interfaceUserManagement.GetMenuRolesByRoleId(roleId);
            return data;
        }

        //[Authorize]
        [HttpGet("GetMenuRoleList")]
        public ServiceResponse<List<RoleMenus>> GetMenuRoleList(int? roleId = null)
        {
            var data = _interfaceUserManagement.GetMenuRoleList(roleId);
            return data;
        }

        //[Authorize]
        [HttpPost("AssignMenuToRole")]
        public ServiceResponse<RoleMenus> AssignMenuToRole(RoleMenus menuRole)
        {
            var data = _interfaceUserManagement.AssignMenuToRole(menuRole);
            return data;
        }

        //[Authorize]
        [HttpPost("SaveMenuRole")]
        public ServiceResponse<RoleMenus> SaveMenuRole(RoleMenus menuRole)
        {
            var data = _interfaceUserManagement.SaveMenuRole(menuRole);
            return data;
        }

        //[Authorize]
        [HttpDelete("DeleteMenuRoleById")]
        public ServiceResponse<bool> DeleteMenuRoleById(int roleId, int menuId)
        {
            var data = _interfaceUserManagement.DeleteMenuRoleById(roleId, menuId);
            return data;
        }

        //[Authorize]
        [HttpDelete("DeleteMenuRole/{menuRoleId}")]
        public ServiceResponse<bool> DeleteMenuRole(int menuRoleId)
        {
            var data = _interfaceUserManagement.DeleteMenuRole(menuRoleId);
            return data;
        }
        #endregion


        #region USER MENU
        //[Authorize]
        [HttpGet("GetUserMenuByUserId")]
        public ServiceResponse<UserMenuList> GetUserMenuByUserId(int userId)
        {
            var data = _interfaceUserManagement.GetUserMenuByUserId(userId);
            return data;
        }


        //[Authorize]
        [HttpGet("GetUserAssignedMenusByRoleId")]
        public ServiceResponse<List<UserAssignedMenus>> GetUserAssignedMenusByRoleId(int roleId)
        {
            var data = _interfaceUserManagement.GetUserAssignedMenusByRoleId(roleId);
            return data;
        }
        #endregion

        #region LOGIN LOG
        //[Authorize]
        [HttpGet("GetUserLoginLogsByUserId")]
        public ServiceResponse<List<VLoginLog>> GetUserLoginLogsByUserId(int userId, string startDate, string endDate)
        {
            var data = _interfaceUserManagement.GetUserLoginLogsByUserId(userId, startDate, endDate);
            return data;
        }

        //[Authorize]
        [HttpGet("GetUserLoginLogsList")]
        public ServiceResponse<List<VLoginLog>> GetUserLoginLogsList(string startDate, string endDate)
        {
            var data = _interfaceUserManagement.GetUserLoginLogsList(startDate, endDate);
            return data;
        }

        //[Authorize]
        [HttpGet("GetUserLastLoginLogsList")]
        public ServiceResponse<List<VLoginLog>> GetUserLastLoginLogsList()
        {
            var data = _interfaceUserManagement.GetUserLastLoginLogsList();
            return data;
        }
        #endregion
    }
} 