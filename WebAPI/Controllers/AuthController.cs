using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BepopStreamProject.DTO_s.Auth;
using BepopStreamProject.Helpers;
using Core.Entities;
using DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BepopStreamProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly BepopDbContext _context;
        private readonly IConfiguration _config;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthController(BepopDbContext context, IConfiguration config, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _config = config;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                ViewBag.Message = "Bu email zaten kayıtlı.";
                return View(dto);
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                MembershipLevel = dto.MembershipLevel > 0 ? dto.MembershipLevel : 1,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                ViewBag.Message = "Hatalı email veya şifre.";
                return View(dto);
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.Name)
                .ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? "User"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("MembershipLevel", user.MembershipLevel.ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (!userRoles.Any()) claims.Add(new Claim(ClaimTypes.Role, "User"));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = dto.RememberMe,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            if (userRoles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Discover");
        }

        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwtToken");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}