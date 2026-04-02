using System.Security.Claims;

namespace BepopStreamProject.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserLevel(this ClaimsPrincipal user)
        {
            var levelValue =
                user.FindFirst("MembershipLevel")?.Value ??
                user.FindFirst("membershipLevel")?.Value ?? 
                "0";

            return int.TryParse(levelValue, out var level) ? level : 0;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            var idValue =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                user.FindFirst("userId")?.Value ??                 
                "0";

            return int.TryParse(idValue, out var id) ? id : 0;
        }
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return
                user.FindFirst(ClaimTypes.Name)?.Value ??   
                user.FindFirst("username")?.Value ??        
                "Misafir";
        }
    }
}
