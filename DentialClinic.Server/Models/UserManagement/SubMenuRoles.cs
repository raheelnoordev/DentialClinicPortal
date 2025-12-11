using System.ComponentModel.DataAnnotations;

namespace DentialClinic.Server.Models.UserManagement
{
    public class SubMenuRoles
    {
        [Key]
        public int? SubMenuRoleId { get; set; }
        public int RoleId { get; set; }
        public int SubMenuId { get; set; }
    }


    public class SubMenuRolesModel
    {
        [Key]
        public int? SubMenuRoleId { get; set; }
        public int RoleId { get; set; }
        public int[]? SubMenuId { get; set; }
    }
}
