using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Branch
{
    [NotMapped]
    public class BranchDto
    {
        //public int BranchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Phone { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public bool IsActive { get; set; }
        //public DateTime? CreatedAt { get; set; }
    }
}

