using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Treatment
{
    [NotMapped]
    public class TreatmentCatalogDto
    {
        public int TreatmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? DefaultPrice { get; set; }
        public int? DefaultDurationMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}

