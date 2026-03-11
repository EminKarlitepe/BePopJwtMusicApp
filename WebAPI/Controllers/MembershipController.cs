using DataAccess.Context;
using BepopStreamProject.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BepopStreamProject.Controllers
{
    [Authorize]
    public class MembershipController : Controller
    {
        private readonly BepopDbContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public MembershipController(BepopDbContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpGet]
        public IActionResult Upgrade()
        {
            var userId = User.GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            int currentLevel = user?.MembershipLevel ?? 1;

            var packages = _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Level)
                .ToList();

            ViewBag.CurrentLevel = currentLevel;
            ViewBag.Packages = packages;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upgrade(int newLevel)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                            User.FindFirstValue("userId") ??
                            User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound();

            var package = _context.Products.FirstOrDefault(p => p.Level == newLevel && p.IsActive);
            if (package == null || newLevel < 1 || newLevel > 3)
                newLevel = 1;

            user.MembershipLevel = newLevel;
            _context.SaveChanges();

            var newToken = _jwtTokenGenerator.GenerateToken(user);
            Response.Cookies.Append("jwtToken", newToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            var existingRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!existingRoles.Any()) existingRoles.Add("User");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("MembershipLevel", user.MembershipLevel.ToString())
            };
            foreach (var role in existingRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            string levelName = newLevel switch { 1 => "Ücretsiz", 2 => "Gold", 3 => "Premium", _ => "Ücretsiz" };
            TempData["Message"] = $"Paketiniz {levelName} olarak güncellendi!";
            return RedirectToAction("Upgrade");
        }
    }
}