using System.ComponentModel.DataAnnotations;
using DentialClinic.Server.Models.Branch;

namespace DentialClinic.Server.Models.WhatsApp
{
    public class WhatsAppScreen
    {
        [Key]
        public int ScreenId { get; set; }
        
        public int? BranchId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = "en";
        
        [MaxLength(100)]
        public string? Title { get; set; }
        
        [Required]
        public string MessageText { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(30)]
        public string ScreenType { get; set; } = string.Empty; // MENU/INFO/QUESTION
        
        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual BranchDto? Branch { get; set; }
        public virtual ICollection<WhatsAppScreenOption> Options { get; set; } = new List<WhatsAppScreenOption>();
    }
}

