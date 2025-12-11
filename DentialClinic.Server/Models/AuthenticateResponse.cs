
using DentialClinic.Server.Models.Company;
using DentialClinic.Server.Models.UserManagement;

namespace DentialClinic.Server.Model
{
    public class AuthenticateResponse
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        
        public string? Email { get; set; }
        public string? DefaultUrl { get; set; }
        public string? Token { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }

      
        public string? ApplicationVersion { get; set; }
        //public string? CanMask { get; set; }
        //public string? CanTamper { get; set; }
        //public string? CanEdit { get; set; }
       

        public AuthenticateResponse(Users? user, string? token, string? roleName = null)
        {
            if (user != null)
            {
                UserId = user.UserId;
                FullName = user.FullName;
                Email = user.Email;
                RoleName = roleName;
                RoleId = user.RoleId;
            }
            Token = token;
        }
    }
}





