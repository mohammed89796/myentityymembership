 using System.Security.Claims;

namespace MyEntityMembershipService.Extensions
{
    public static class TenantExtensions
    {
        public static string? GetTenantId(this ClaimsPrincipal user) =>
             user.FindFirstValue("tenantId");


        public static int? GetParsedTenantId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue("tenantId");
            return int.TryParse(value, out var parsed) ? parsed : null;
        }
    }
}
