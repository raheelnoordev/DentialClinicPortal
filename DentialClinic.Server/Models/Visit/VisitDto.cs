using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Visit
{
    [NotMapped]
    public class VisitDto
    {
        public int VisitId { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int? AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime VisitTime { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "OPEN";
        public List<VisitTreatmentDto> Treatments { get; set; } = new List<VisitTreatmentDto>();
    }

    public class VisitTreatmentDto
    {
        public int VisitTreatmentId { get; set; }
        public int TreatmentId { get; set; }
        public string TreatmentName { get; set; } = string.Empty;
        public string? ToothNumber { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Notes { get; set; }
    }
}

