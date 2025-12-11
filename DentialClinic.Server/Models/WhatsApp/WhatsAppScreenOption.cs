using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.WhatsApp;
using DentialClinic.Server.Models.Treatment;

namespace DentialClinic.Server.Models.WhatsApp
{
    public class WhatsAppScreenOption
    {
        [Key]
        public int OptionId { get; set; }
        
        [Required]
        public int ScreenId { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string Label { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string PayloadValue { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? NextScreenCode { get; set; }
        
        public int? TreatmentId { get; set; }
        
        [Required]
        public int SortOrder { get; set; } = 1;
        
        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual WhatsAppScreen Screen { get; set; } = null!;
        public virtual TreatmentCatalog? Treatment { get; set; }
    }
}

