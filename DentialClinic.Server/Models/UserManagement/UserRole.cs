using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.UserManagement
{
    [Table("user_roles")]
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int RoleId { get; set; }

        // Navigation properties
        public virtual Users? User { get; set; }
        public virtual Roles? Role { get; set; }
    }
}
