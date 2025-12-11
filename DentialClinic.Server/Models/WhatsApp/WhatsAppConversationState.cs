using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace DentialClinic.Server.Models.WhatsApp
{
    public class WhatsAppConversationState
    {
        [Key]
        public int ConversationId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FromNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string CurrentScreenCode { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = "en";
        
        [Column(TypeName = "jsonb")]
        public string? DataJson { get; set; }
        
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

