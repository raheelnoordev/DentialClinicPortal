using DentialClinic.Server.Models.UserManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Branch
{
    [Table("branches")]
    public class Branch
    {
        [Key]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("code")]
        public string? Code { get; set; }

        [MaxLength(50)]
        [Column("phone")]
        public string? Phone { get; set; }

        [MaxLength(50)]
        [Column("whatsapp_number")]
        public string? WhatsAppNumber { get; set; }

        [MaxLength(150)]
        [Column("email")]
        public string? Email { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [MaxLength(100)]
        [Column("city")]
        public string? City { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        //public virtual ICollection<Users> Users { get; set; } = new List<Users>();

        //public virtual Patient.Patient Patients { get; set; } = new Patient.Patient();
        //public virtual Doctor.Doctor Doctors { get; set; } = new Doctor.Doctor();
    }
}
