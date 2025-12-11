using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Patient
{
    [NotMapped]
    public class PatientDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public int BranchId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}

