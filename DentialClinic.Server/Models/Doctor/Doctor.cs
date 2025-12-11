using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Doctor
{
    [Table("doctors")]
    public class Doctor
    {
        [Key]
        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [MaxLength(100)]
        [Column("speciality")]
        public string? Speciality { get; set; }

        // PostgreSQL type is numeric(12,2)
        [Column("default_fee", TypeName = "numeric(12,2)")]
        public decimal? DefaultFee { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        //public virtual User User { get; set; } = null!;
        //public virtual Branch.Branch Branch { get; set; }   
        ////public virtual Branch Branch { get; set; }
        //public virtual Patient.Patient Patients { get; set; } = new Patient.Patient();
        //public virtual Appointment.Appointment Appointments { get; set; } = new Appointment.Appointment();
        //public virtual Visit.Visit Visits { get; set; } = new Visit.Visit();
    }
}

