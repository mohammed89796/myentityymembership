using System.Security.Claims;
using System.Text.Json;

namespace MyEntityMembershipService.Extensions
{
    public static class UserExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static List<string> GetRoles(this ClaimsPrincipal user)
        {
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            return roles;
        }


        public static string? GetUserName(this ClaimsPrincipal user) =>
            user.FindFirstValue("name");

        public static bool IsSuperAdmin(this ClaimsPrincipal user) =>
            user.GetRoles().Contains("SuperAdmin", StringComparer.OrdinalIgnoreCase);

        public static bool IsTenantAdmin(this ClaimsPrincipal user) =>
            user.GetRoles().Contains("TenantAdmin", StringComparer.OrdinalIgnoreCase);
    }
}
