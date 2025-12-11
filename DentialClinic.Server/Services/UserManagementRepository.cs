using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models.Company;
using DentialClinic.Server.Models.UserManagement;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DentialClinic.Server.Repository
{
    public class UserManagmentRepository : IUserManagement
    {
        private readonly ApplicationDbContext _context;
        //private readonly IUserManagement _userManagementRepository;

        public UserManagmentRepository(ApplicationDbContext context)
        {
            _context = context;
            //_userManagementRepository = userManagementRepository;
        }

        #region USER
        public ServiceResponse<List<Users>> GetUsersList()
        {
            var response = new ServiceResponse<List<Users>>();
            try
            {
                var users = _context.Users.ToList();
                response.Result = users;
                response.Success = true;
                response.Message = "Users retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<Users> GetUserById(int userId)
        {
            var response = new ServiceResponse<Users>();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    response.Result = user;
                    response.Success = true;
                    response.Message = "User retrieved successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        //public ServiceResponse<Users> SaveUser(Users user)
        //{
        //    var response = new ServiceResponse<Users>();
        //    try
        //    {
        //        //user.CreatedAt = DateTime.Now;
        //        //user.UpdatedAt = DateTime.Now;
        //        user.CreatedAt = DateTime.UtcNow;
        //        user.UpdatedAt = DateTime.UtcNow;


        //        if (!string.IsNullOrEmpty(user.PasswordHash))
        //        {
        //            user.PasswordHash = HashPassword(user.PasswordHash);
        //        }

        //        _context.Users.Add(user);
        //        _context.SaveChanges();

        //        response.Result = user;
        //        response.Success = true;
        //        response.Message = "User saved successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = ex.Message;
        //    }
        //    return response;
        //}

        public ServiceResponse<Users> SaveUser(Users user)
        {
            var response = new ServiceResponse<Users>();
            try
            {
                // Set timestamps
                user.CreatedAt = DateTime.UtcNow;
                //user.UpdatedAt = DateTime.UtcNow;

                // Hash password if provided
                //if (!string.IsNullOrEmpty(user.PasswordHash))
                //{
                //    user.PasswordHash = HashPassword(user.PasswordHash);
                //}

                // Save user first
                _context.Users.Add(user);
                _context.SaveChanges();

                // Handle customer assignments if provided
                

                response.Result = user;
                response.Success = true;
                response.Message = "User saved successfully with customer assignments";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<Users> UpdateUser(Users user)
        {
            var response = new ServiceResponse<Users>();
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
                if (existingUser != null)
                {
                    existingUser.BranchId = user.BranchId;
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.IsActive = user.IsActive;
                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        existingUser.PasswordHash = user.PasswordHash;
                    }

                    //if (!string.IsNullOrEmpty(user.PasswordHash))
                    //{
                    //    existingUser.PasswordHash = HashPassword(user.PasswordHash);
                    //}

                    _context.SaveChanges();

                    response.Result = existingUser;
                    response.Success = true;
                    response.Message = "User updated successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> DeleteUserById(int userId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();

                    response.Result = true;
                    response.Success = true;
                    response.Message = "User deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> DeactivateUserById(int userId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    user.IsActive = false;
                    _context.SaveChanges();

                    response.Result = true;
                    response.Success = true;
                    response.Message = "User deactivated successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId && u.PasswordHash == oldPassword);
                if (user != null)
                {
                    //var hashedOldPassword = oldPassword;
                    if (user.PasswordHash == oldPassword)
                    {
                        user.PasswordHash = newPassword;
                        //user.UpdatedAt = DateTime.UtcNow;
                        _context.SaveChanges();

                        response.Result = true;
                        response.Success = true;
                        response.Message = "Password changed successfully";
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Old password is incorrect";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> ResetPassword(int userId, string newPassword)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    user.PasswordHash = newPassword;
                    //user.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();

                    response.Result = true;
                    response.Success = true;
                    response.Message = "Password reset successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region ROLE
        public ServiceResponse<List<Roles>> GetRoleList()
        {
            var response = new ServiceResponse<List<Roles>>();
            try
            {
                var roles = _context.Roles.ToList();
                response.Result = roles;
                response.Success = true;
                response.Message = "Roles retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<Roles> GetRoleById(int roleId)
        {
            var response = new ServiceResponse<Roles>();
            try
            {
                var role = _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
                if (role != null)
                {
                    response.Result = role;
                    response.Success = true;
                    response.Message = "Role retrieved successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Role not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<Roles> SaveRole(Roles role)
        {
            var response = new ServiceResponse<Roles>();
            try
            {
                //role.CreatedAt = DateTime.UtcNow;
                //role.UpdatedAt = DateTime.UtcNow;

                _context.Roles.Add(role);
                _context.SaveChanges();

                response.Result = role;
                response.Success = true;
                response.Message = "Role saved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<Roles> UpdateRole(Roles role)
        {
            var response = new ServiceResponse<Roles>();
            try
            {
                var existingRole = _context.Roles.FirstOrDefault(r => r.RoleId == role.RoleId);
                if (existingRole != null)
                {
                    existingRole.RoleName = role.RoleName;

                    _context.SaveChanges();

                    response.Result = existingRole;
                    response.Success = true;
                    response.Message = "Role updated successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Role not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> DeleteRoleById(int roleId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var role = _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
                if (role != null)
                {
                    _context.Roles.Remove(role);
                    _context.SaveChanges();

                    response.Result = true;
                    response.Success = true;
                    response.Message = "Role deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Role not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region MENU
        public ServiceResponse<List<Menus>> GetMenuList()
        {
            var response = new ServiceResponse<List<Menus>>();
            try
            {
                var menus = _context.Menus.ToList();
                response.Result = menus;
                response.Success = true;
                response.Message = "Menus retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<Menus> GetMenuById(int menuId)
        {
            var response = new ServiceResponse<Menus>();
            try
            {
                var menu = _context.Menus.FirstOrDefault(m => m.MenuId == menuId);
                if (menu != null)
                {
                    response.Result = menu;
                    response.Success = true;
                    response.Message = "Menu retrieved successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Menu not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<Menus> SaveMenu(Menus menu)
        {
            var response = new ServiceResponse<Menus>();
            try
            {
                //menu.CreatedAt = DateTime.UtcNow;
                //menu.UpdatedAt = DateTime.UtcNow;

                _context.Menus.Add(menu);
                _context.SaveChanges();

                response.Result = menu;
                response.Success = true;
                response.Message = "Menu saved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<Menus> UpdateMenu(Menus menu)
        {
            var response = new ServiceResponse<Menus>();
            try
            {
                var existingMenu = _context.Menus.FirstOrDefault(m => m.MenuId == menu.MenuId);
                if (existingMenu != null)
                {
                    existingMenu.Name = menu.Name;
                    existingMenu.Url = menu.Url;
                    existingMenu.Icon = menu.Icon;
                    existingMenu.SortOrder = menu.SortOrder;
                    existingMenu.IsActive = menu.IsActive;

                    _context.SaveChanges();

                    response.Result = existingMenu;
                    response.Success = true;
                    response.Message = "Menu updated successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Menu not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> DeleteMenuById(int menuId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var menu = _context.Menus.FirstOrDefault(m => m.MenuId == menuId);
                if (menu != null)
                {
                    _context.Menus.Remove(menu);
                    _context.SaveChanges();

                    response.Result = true;
                    response.Success = true;
                    response.Message = "Menu deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Menu not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region USER ROLE
        public ServiceResponse<List<UserRole>> GetUserRolesByUserId(int userId)
        {
            var response = new ServiceResponse<List<UserRole>>();
            try
            {
                var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();
                response.Result = userRoles;
                response.Success = true;
                response.Message = "User roles retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<UserRole> AssignUserRole(UserRole userRole)
        {
            var response = new ServiceResponse<UserRole>();
            try
            {
                //userRole.CreatedOn = DateTime.Now;
                //userRole.UpdatedOn = DateTime.Now;

                _context.UserRoles.Add(userRole);
                _context.SaveChanges();

                response.Result = userRole;
                response.Success = true;
                response.Message = "User role assigned successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> DeleteUserRoleById(int userId, int roleId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);
                if (userRole != null)
                {
                    _context.UserRoles.Remove(userRole);
                    _context.SaveChanges();

                    response.Result = true;
                    response.Success = true;
                    response.Message = "User role deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "User role not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion


        #region MENU ROLE
        public ServiceResponse<List<RoleMenus>> GetMenuRolesByRoleId(int roleId)
        {
            var response = new ServiceResponse<List<RoleMenus>>();
            try
            {
                var menuRoles = _context.RoleMenus.Where(mr => mr.RoleId == roleId).ToList();
                response.Result = menuRoles;
                response.Success = true;
                response.Message = "Menu roles retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<List<RoleMenus>> GetMenuRoleList(int? roleId = null)
        {
            var response = new ServiceResponse<List<RoleMenus>>();
            try
            {
                var query = _context.RoleMenus.AsQueryable();
                
                if (roleId.HasValue)
                {
                    query = query.Where(mr => mr.RoleId == roleId.Value);
                }
                
                var menuRoles = query.ToList();
                response.Result = menuRoles;
                response.Success = true;
                response.Message = "Menu roles retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<RoleMenus> AssignMenuToRole(RoleMenus menuRole)
        {
            var response = new ServiceResponse<RoleMenus>();
            try
            {
                _context.RoleMenus.Add(menuRole);
                _context.SaveChanges();

                response.Result = menuRole;
                response.Success = true;
                response.Message = "Menu assigned to role successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<RoleMenus> SaveMenuRole(RoleMenus menuRole)
        {
            var response = new ServiceResponse<RoleMenus>();
            try
            {
                // Check if menu role already exists
                var existingMenuRole = _context.RoleMenus.FirstOrDefault(mr => mr.RoleId == menuRole.RoleId && mr.MenuId == menuRole.MenuId);
                
                if (existingMenuRole != null)
                {
                    response.Result = existingMenuRole;
                    response.Success = true;
                    response.Message = "Menu role already exists";
                }
                else
                {
                    _context.RoleMenus.Add(menuRole);
                    _context.SaveChanges();

                    response.Result = menuRole;
                    response.Success = true;
                    response.Message = "Menu role saved successfully";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> DeleteMenuRoleById(int roleId, int menuId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var menuRole = _context.RoleMenus.FirstOrDefault(mr => mr.RoleId == roleId && mr.MenuId == menuId);
                if (menuRole != null)
                {
                    _context.RoleMenus.Remove(menuRole);
                    _context.SaveChanges();

                    response.Result = true;
                    response.Success = true;
                    response.Message = "Menu role deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Menu role not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<bool> DeleteMenuRole(int menuRoleId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var menuRole = _context.RoleMenus.FirstOrDefault(mr => mr.RoleMenuId == menuRoleId);
                if (menuRole != null)
                {
                    _context.RoleMenus.Remove(menuRole);
                    _context.SaveChanges();

                    response.Result = true;
                    response.Success = true;
                    response.Message = "Menu role deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Menu role not found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion


        #region USER MENU
        public ServiceResponse<UserMenuList> GetUserMenuByUserId(int userId)
        {
            var response = new ServiceResponse<UserMenuList>();
            try
            {
                // This would need to be implemented based on your UserMenuList model
                response.Success = false;
                response.Message = "Method not implemented";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }


        public ServiceResponse<List<UserAssignedMenus>> GetUserAssignedMenusByRoleId(int roleId)
        {
            var response = new ServiceResponse<List<UserAssignedMenus>>();
            try
            {
                // Get menus assigned to this role through RoleMenus table and join with Menus table
                var assignedMenus = (from mr in _context.RoleMenus
                                   join m in _context.Menus on mr.MenuId equals m.MenuId
                                   where mr.RoleId == roleId && m.IsActive
                                   orderby m.SortOrder
                                   select new UserAssignedMenus
                                   {
                                       MenuId = m.MenuId,
                                       MenuName = m.Name,
                                       IconUrl = m.Icon,
                                       Link = m.Url,
                                       OrderNo = m.SortOrder,
                                       IsEnabled = m.IsActive ? "Y" : "N",
                                       RoleId = mr.RoleId
                                   }).ToList();

                response.Result = assignedMenus;
                response.Success = true;
                response.Message = $"Retrieved {assignedMenus.Count} assigned menus for role {roleId}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region LOGIN LOG
        //public ServiceResponse<List<VLoginLog>> GetUserLoginLogsByUserId(int userId, string startDate, string endDate)
        //{
        //    var response = new ServiceResponse<List<VLoginLog>>();
        //    try
        //    {
        //        var loginLogs = _context.VLoginLogs.Where(ll => ll.UserId == userId).ToList();
        //        response.Result = loginLogs;
        //        response.Success = true;
        //        response.Message = "Login logs retrieved successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = ex.Message;
        //    }
        //    return response;
        //}

        public ServiceResponse<List<VLoginLog>> GetUserLoginLogsList(string startDate, string endDate)
        {
            var response = new ServiceResponse<List<VLoginLog>>();
            try
            {
                var loginLogs = _context.Users.ToList();
                //response.Result = loginLogs;
                response.Success = true;
                response.Message = "Login logs retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public ServiceResponse<List<VLoginLog>> GetUserLastLoginLogsList()
        {
            var response = new ServiceResponse<List<VLoginLog>>();
            try
            {
                var loginLogs = _context.Users.ToList();
                //response.Result = loginLogs;
                response.Success = true;
                response.Message = "Last login logs retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public ServiceResponse<List<VLoginLog>> GetUserLoginLogsByUserId(int userId, string startDate, string endDate)
        {
            throw new NotImplementedException();
        }
    }
}