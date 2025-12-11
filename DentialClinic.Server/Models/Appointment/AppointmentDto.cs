using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Appointment
{
    [NotMapped]
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string? PatientPhone { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public DateTime AppointmentAt { get; set; }
        public int DurationMin { get; set; } = 10;
        public string Status { get; set; } = "booked";
        public string Source { get; set; } = "call";
        public string? Reason { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

