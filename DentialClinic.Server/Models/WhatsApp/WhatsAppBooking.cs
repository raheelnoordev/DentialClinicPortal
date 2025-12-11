using System.ComponentModel.DataAnnotations;
using DentialClinic.Server.Models.Appointment;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.Patient;

namespace DentialClinic.Server.Models.WhatsApp
{
    public class WhatsAppBooking
    {
        [Key]
        public int WhatsAppBookingId { get; set; }
        
        public int? BranchId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FromNumber { get; set; } = string.Empty;
        
        [Required]
        public string MessageText { get; set; } = string.Empty;
        
        [Required]
        public DateTime ReceivedAt { get; set; } = DateTime.Now;
        
        [MaxLength(150)]
        public string? ParsedPatientName { get; set; }
        
        public DateTime? ParsedPreferredTime { get; set; }
        
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "NEW"; // NEW/LINKED/IGNORED
        
        public int? PatientId { get; set; }

        // Navigation properties
        public virtual BranchDto? Branch { get; set; }
        public virtual PatientDto? Patient { get; set; }
        public virtual ICollection<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
    }
}

