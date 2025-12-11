using System.ComponentModel.DataAnnotations;

namespace DentialClinic.Server.Models.UserManagement
{
    public class LoginLog
    {
        [Key]
        public int? LogID { get; set; }
        public int? UserID { get; set; }
        public DateTime? LoginTime { get; set; }
        public string? IPAddress { get; set; }
    }
}
