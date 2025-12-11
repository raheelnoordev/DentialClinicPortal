using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DentialClinic.Server.Models.Visit;
using DentialClinic.Server.Models.Treatment;

namespace DentialClinic.Server.Models.Visit
{
    [Table("visit_treatments")]
    public class VisitTreatment
    {
        [Key]
        public int VisitTreatmentId { get; set; }
        
        [Required]
        public int VisitId { get; set; }
        
        [Required]
        public int TreatmentId { get; set; }
        
        [MaxLength(10)]
        public string? ToothNumber { get; set; }
        
        [Required]
        public int Quantity { get; set; } = 1;
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal DiscountAmount { get; set; } = 0;
        
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Visit Visit { get; set; } = null!;
        public virtual TreatmentCatalog Treatment { get; set; } = null!;
    }
}

