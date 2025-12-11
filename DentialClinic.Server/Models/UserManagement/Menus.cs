using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.UserManagement
{
    [Table("menus")]
    public class Menus
    {
        [Key]
        public int MenuId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(150)]
        public string? Url { get; set; }
        
        [MaxLength(100)]
        public string? Icon { get; set; }
        
        [Required]
        public int SortOrder { get; set; } = 1;
        
        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<RoleMenus> RoleMenus { get; set; } = new List<RoleMenus>();
    }
} 