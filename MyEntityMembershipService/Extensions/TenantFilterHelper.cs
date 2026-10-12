using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Extensions
{
    public static class TenantFilterHelper
    {
        public static bool IsTenantAllowed(ITenantProvider provider, int tenantId)
        {
            // Called per record during query execution
            if (provider.IgnoreTenantFilter) return true;
            return provider.AllowedTenantIds?.Contains(tenantId) ?? false;
        }
    }



}
