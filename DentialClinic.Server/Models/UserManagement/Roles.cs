using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.UserManagement
{
    [Table("roles")]
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<RoleMenus> RoleMenus { get; set; } = new List<RoleMenus>();
    }
}
