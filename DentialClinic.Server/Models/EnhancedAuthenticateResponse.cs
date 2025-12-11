using DentialClinic.Server.Model;
using DentialClinic.Server.Models.UserManagement;

namespace DentialClinic.Server.Models
{
    public class EnhancedAuthenticateResponse
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? DefaultUrl { get; set; }
        public string? Token { get; set; }
        public int? EmpId { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? ApplicationVersion { get; set; }
        //public string? CanMask { get; set; }
        //public string? CanTamper { get; set; }
        //public string? CanEdit { get; set; }
        public bool? CanPurchaseMaterial { get; set; }
        public bool? AllowDispatch { get; set; }
        public bool? AllowReceiving { get; set; }
        public List<UserAssignedMenus> AssignedMenus { get; set; } = new List<UserAssignedMenus>();

        public EnhancedAuthenticateResponse()
        {
        }

        public EnhancedAuthenticateResponse(AuthenticateResponse baseResponse, List<UserAssignedMenus> assignedMenus)
        {
            UserId = baseResponse.UserId;
            FirstName = baseResponse.FullName;
            Username = baseResponse.Email;
            DefaultUrl = baseResponse.DefaultUrl;
            Token = baseResponse.Token;
           
            RoleId = baseResponse.RoleId;
            RoleName = baseResponse.RoleName;
            ApplicationVersion = baseResponse.ApplicationVersion;
            //CanMask = baseResponse.CanMask;
            //CanTamper = baseResponse.CanTamper;
            //CanEdit = baseResponse.CanEdit;
            
            AssignedMenus = assignedMenus;
        }
    }
}
