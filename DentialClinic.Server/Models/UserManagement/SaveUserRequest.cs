using System.ComponentModel.DataAnnotations;

namespace DentialClinic.Server.Models.UserManagement
{
    public class SaveUserRequest
    {
        public int? BranchId { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Phone { get; set; }
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public int RoleId { get; set; }
    }
}
