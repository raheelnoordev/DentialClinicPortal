using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Doctor
{
    public class DoctorDto
    {
        //public int DoctorId { get; set; }
        public int UserId { get; set; }
        //public string UserName { get; set; } = string.Empty;
        //public string UserEmail { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        //public string? BranchName { get; set; }
        public string? Speciality { get; set; }
        public decimal? DefaultFee { get; set; }
        public bool IsActive { get; set; }
    }
}

