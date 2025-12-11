namespace DentialClinic.Server.Models
{
    public class ResetPasswordDto
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
