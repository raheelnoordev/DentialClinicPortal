using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.UserManagement
{
    [Table("role_menus")]
    public class RoleMenus
    {
        [Key]
        public int RoleMenuId { get; set; }
        
        [Required]
        public int RoleId { get; set; }
        
        [Required]
        public int MenuId { get; set; }

        // Navigation properties
        public virtual Roles? Role { get; set; }
        public virtual Menus? Menu { get; set; }
    }
}

