using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.Branch;
using DentialClinic.Server.Models.Patient;
using DentialClinic.Server.Models.Doctor;
using DentialClinic.Server.Models.UserManagement;

namespace DentialClinic.Server.Models.Appointment
{
    [Table("appointments")]
    public class Appointment
    {
        [Key]
        [Column("appointment_id")]
        public int AppointmentId { get; set; }

        [Required]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [Required]
        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Required]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [Required]
        [Column("appointment_at")]
        public DateTime AppointmentAt { get; set; }

        [Required]
        [Column("duration_min")]
        public int DurationMin { get; set; } = 10;

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "booked";

        [Required]
        [MaxLength(20)]
        [Column("source")]
        public string Source { get; set; } = "call";

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("created_by_user_id")]
        public int? CreatedByUserId { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Branch.Branch Branch { get; set; } = null!;
        public virtual Patient.Patient Patient { get; set; } = null!;
        public virtual Doctor.Doctor Doctor { get; set; } = null!;
        public virtual Users? CreatedByUser { get; set; }
    }
}

