using DentialClinic.Api.Model;
using DentialClinic.Server.Models.UserManagement;

namespace DentialClinic.Server.Interfaces
{
    public interface IUserManagement
    {
        #region USER
        ServiceResponse<List<Users>> GetUsersList();
        ServiceResponse<Users> GetUserById(int userId);
        ServiceResponse<Users> SaveUser(Users user);
        ServiceResponse<Users> UpdateUser(Users user);
        ServiceResponse<bool> DeleteUserById(int userId);
        ServiceResponse<bool> DeactivateUserById(int userId);
        ServiceResponse<bool> ChangePassword(int userId, string oldPassword, string newPassword);
        ServiceResponse<bool> ResetPassword(int userId, string newPassword);
        #endregion

        #region ROLE
        ServiceResponse<List<Roles>> GetRoleList();
        ServiceResponse<Roles> GetRoleById(int roleId);
        ServiceResponse<Roles> SaveRole(Roles role);
        ServiceResponse<Roles> UpdateRole(Roles role);
        ServiceResponse<bool> DeleteRoleById(int roleId);
        #endregion

        #region MENU
        ServiceResponse<List<Menus>> GetMenuList();
        ServiceResponse<Menus> GetMenuById(int menuId);
        ServiceResponse<Menus> SaveMenu(Menus menu);
        ServiceResponse<Menus> UpdateMenu(Menus menu);
        ServiceResponse<bool> DeleteMenuById(int menuId);
        #endregion

        #region USER ROLE
        ServiceResponse<List<UserRole>> GetUserRolesByUserId(int userId);
        ServiceResponse<UserRole> AssignUserRole(UserRole userRole);
        ServiceResponse<bool> DeleteUserRoleById(int userId, int roleId);
        #endregion

        #region MENU ROLE
        ServiceResponse<List<RoleMenus>> GetMenuRolesByRoleId(int roleId);
        ServiceResponse<List<RoleMenus>> GetMenuRoleList(int? roleId = null);
        ServiceResponse<RoleMenus> AssignMenuToRole(RoleMenus menuRole);
        ServiceResponse<RoleMenus> SaveMenuRole(RoleMenus menuRole);
        ServiceResponse<bool> DeleteMenuRoleById(int roleId, int menuId);
        ServiceResponse<bool> DeleteMenuRole(int menuRoleId);
        #endregion

        #region USER MENU
        ServiceResponse<UserMenuList> GetUserMenuByUserId(int userId);
        ServiceResponse<List<UserAssignedMenus>> GetUserAssignedMenusByRoleId(int roleId);
        #endregion

        #region LOGIN LOG
        ServiceResponse<List<VLoginLog>> GetUserLoginLogsByUserId(int userId, string startDate, string endDate);
        ServiceResponse<List<VLoginLog>> GetUserLoginLogsList(string startDate, string endDate);
        ServiceResponse<List<VLoginLog>> GetUserLastLoginLogsList();
        #endregion
    }
}
