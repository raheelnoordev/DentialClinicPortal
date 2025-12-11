using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.Patient;
using DentialClinic.Server.Models.Doctor;
using DentialClinic.Server.Models.Appointment;
using DentialClinic.Server.Models.Billing;

namespace DentialClinic.Server.Models.Visit
{
    [Table("visits")]
    public class Visit
    {
        [Key]
        public int VisitId { get; set; }
        
        [Required]
        public int BranchId { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public DateTime VisitTime { get; set; } = DateTime.Now;
        
        public string? Diagnosis { get; set; }
        
        public string? Notes { get; set; }
        
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "OPEN"; // OPEN/CLOSED

        // Navigation properties
        public virtual Branch.Branch Branch { get; set; } = null!;
        public virtual Appointment.Appointment Appointment { get; set; }
        public virtual Patient.Patient Patient { get; set; } = null!;
        public virtual Doctor.Doctor Doctor { get; set; } = null!;
        public virtual ICollection<VisitTreatment> VisitTreatments { get; set; } = new List<VisitTreatment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}

