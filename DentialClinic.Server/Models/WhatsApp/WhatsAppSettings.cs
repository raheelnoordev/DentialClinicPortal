using System.ComponentModel.DataAnnotations;
using DentialClinic.Server.Models.Branch;

namespace DentialClinic.Server.Models.WhatsApp
{
    public class WhatsAppSettings
    {
        [Key]
        public int SettingId { get; set; }
        
        public int? BranchId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string PhoneNumberId { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? BusinessAccountId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string ApiBaseUrl { get; set; } = string.Empty;
        
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        
        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual BranchDto? Branch { get; set; }
    }
}

