using System.Security.Claims;

namespace BepopStreamProject.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserLevel(this ClaimsPrincipal user)
        {
            // Önce cookie tabanlı MVC oturumu için kullanılan claim'leri dene
            var levelValue =
                user.FindFirst("MembershipLevel")?.Value ?? // Cookie'de kullanılan ad
                user.FindFirst("membershipLevel")?.Value ?? // JWT içinde kullanılan ad
                "0";

            return int.TryParse(levelValue, out var level) ? level : 0;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            // Hem JWT hem de Cookie oturumu için olası tüm claim adlarını dene
            var idValue =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? // Standart ASP.NET kimlik claim'i
                user.FindFirst("userId")?.Value ??                 // JWT'de kullanılan özel claim
                "0";

            return int.TryParse(idValue, out var id) ? id : 0;
        }
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return
                user.FindFirst(ClaimTypes.Name)?.Value ??   // Cookie'de kullanılan standart ad
                user.FindFirst("username")?.Value ??        // JWT'de kullanılan özel ad
                "Misafir";
        }
    }
}
