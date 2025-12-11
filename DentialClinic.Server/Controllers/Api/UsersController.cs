using DentialClinic.Server.Data;
using DentialClinic.Server.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using DentialClinic.Server.Models.UserManagement;
using DentialClinic.Server.Interfaces;
using System.Security.Cryptography;

namespace DentialClinic.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //private IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(ApplicationDbContext context, IConfiguration configuration)
        {
            //_userService = userService;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            try
            {
                // Find user by email only
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    return BadRequest(new { message = "Invalid email or password" });
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return BadRequest(new { message = "User account is inactive. Please contact administrator." });
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);
                var roleName = await GetRoleNameAsync(user.RoleId);
                // Create base response
                var baseResponse = new AuthenticateResponse(user, token, roleName);

                var assignedMenus = await GetUserAssignedMenusByRoleAsync(user.RoleId);

                // Create enhanced response with customers and assigned menus
                var enhancedResponse = new EnhancedAuthenticateResponse(baseResponse, assignedMenus);
                return Ok(enhancedResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Authentication failed: {ex.Message}" });
            }
        }
        private async Task<List<UserAssignedMenus>> GetUserAssignedMenusByRoleAsync(int roleId)
        {
            try
            {
                // Use the new API endpoint to get assigned menus
                var assignedMenus = await _context.RoleMenus
                    .Where(mr => mr.RoleId == roleId)
                    .Join(_context.Menus,
                          mr => mr.MenuId,
                          m => m.MenuId,
                          (mr, m) => new UserAssignedMenus
                          {
                              MenuId = m.MenuId,
                              MenuName = m.Name,
                              IconUrl = m.Icon,
                              Link = m.Url,
                              OrderNo = m.SortOrder,
                              RoleId = mr.RoleId
                          })
                    .ToListAsync();

                return assignedMenus;
            }
            catch (Exception ex)
            {
                // Log the exception in production
                // For now, return empty list if there's an error
                return new List<UserAssignedMenus>();
            }
        }
        private async Task<string?> GetRoleNameAsync(int? roleId)
        {
            try
            {
                if (roleId == null || roleId == 0)
                    return null;

                var role = await _context.Roles
                    .Where(r => r.RoleId == roleId)
                    .FirstOrDefaultAsync();

                return role?.RoleName;
            }
            catch (Exception ex)
            {
                // Log the exception in production
                // For now, return null if there's an error
                return null;
            }
        }
        private string GenerateJwtToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Email ?? ""),
                    new Claim(ClaimTypes.GivenName, user.FullName ?? ""),
                }),
                Expires = DateTime.UtcNow.AddHours(24), // 24 hours expiration
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}