using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.Branch;

namespace DentialClinic.Server.Models.UserManagement
{
    [Table("users")]
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        
        public int? BranchId { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Phone { get; set; }
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int RoleId { get; set; }
        // Navigation properties
        //public virtual BranchDto? Branchches { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
} 