using Core.Entities;
using DataAccess.Context;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BepopStreamProject.Helpers
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _config;
        private readonly BepopDbContext _context;

        public JwtTokenGenerator(IConfiguration config, BepopDbContext context)
        {
            _config = config;
            _context = context;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            // Kullanýcýnýn rollerini veritabanýndan çek
            var userRoles = _context.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.Name)
                .ToList();

            var claims = new List<Claim>
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim("username", user.Username),
                new Claim("email", user.Email),
                new Claim("membershipLevel", user.MembershipLevel.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Rolleri claim olarak ekle
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Hiç rol yoksa varsayýlan User ekle
            if (!userRoles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}