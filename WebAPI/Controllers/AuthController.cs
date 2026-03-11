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
            // Kullanıcıyı ve rollerini tek seferde çekelim
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                ViewBag.Message = "Hatalı email veya şifre.";
                return View(dto);
            }

            // 1. JWT İşlemleri (API istekleri için cookie'ye ekle)
            var token = _jwtTokenGenerator.GenerateToken(user);
            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // HTTPS kullanıyorsan true kalmalı
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            // 2. Rolleri Getir
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.Name)
                .ToListAsync();

            // 3. Claims Oluştur (ÖNEMLİ: Name ve Identifier boş olmamalı)
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

            // 4. ClaimsIdentity ve Principal Oluştur
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 5. MVC Oturumunu Aç (Kritik nokta burası)
            var authProperties = new AuthenticationProperties
            {
                // Kullanıcının "Beni Hatırla" seçimine göre kalıcı oturum ayarla
                IsPersistent = dto.RememberMe,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            // 6. Yönlendirme (Admin değilse doğrudan Discover'a gönder ki login döngüsünden çıksın)
            if (userRoles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            // Home/Index bazen boş olabiliyor, direkt Keşfet'e gönderelim
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