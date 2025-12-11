using DentialClinic.Server.Models.Appointment;
using DentialClinic.Server.Models.Visit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Treatment
{
    [Table("treatment_catalog")]
    public class TreatmentCatalog
    {
        [Key]
        public int TreatmentId { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Column(TypeName = "decimal(12,2)")]
        public decimal? DefaultPrice { get; set; }
        
        public int? DefaultDurationMinutes { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<VisitTreatment> VisitTreatments { get; set; } = new List<VisitTreatment>();
        //public virtual ICollection<App> Appointments { get; set; } = new List<Appointment>();
    }
}

