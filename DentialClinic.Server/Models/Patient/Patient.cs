using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Patient
{
    [Table("patients")]
    public class Patient
    {
        [Key]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(10)]
        [Column("gender")]
        public string? Gender { get; set; }

        [Column("date_of_birth", TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Required]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

