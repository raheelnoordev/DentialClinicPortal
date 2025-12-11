using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Doctor
{
    [Table("v_doctor_details")]
    public class VDoctorDetails
    {
        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("doctor_name")]
        public string DoctorName { get; set; } = string.Empty;

        [Column("email")]
        public string? Email { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("branch_name")]
        public string? BranchName { get; set; }

        [Column("speciality")]
        public string? Speciality { get; set; }

        [Column("default_fee")]
        public decimal? DefaultFee { get; set; }

        [Column("doctor_is_active")]
        public bool DoctorIsActive { get; set; }

        [Column("user_is_active")]
        public bool UserIsActive { get; set; }

        [Column("user_created_at")]
        public DateTime UserCreatedAt { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("total_patients")]
        public int TotalPatients { get; set; }
    }
}

