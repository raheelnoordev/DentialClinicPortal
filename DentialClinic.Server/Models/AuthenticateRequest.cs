using System.ComponentModel;

namespace DentialClinic.Server.Model
{
    //public class AuthenticateRequest
    //{
    //    [DefaultValue("System")]
    //    public required string Username { get; set; }

    //    [DefaultValue("System")]
    //    public required string Password { get; set; }

    //    [DefaultValue("1")]
    //    public int EmpId { get; set; }
    //}
    public class AuthenticateRequest
    {
        [DefaultValue("System")]
        public required string Email { get; set; }

        [DefaultValue("System")]
        public required string Password { get; set; }


    }

}



