using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Extensions
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider, EntityMembershipDbContext dbContext)
        {
            var user = context.User;
            var tenantIdClaim = user?.FindFirst("tenantId")?.Value;

            if (tenantIdClaim != null && int.TryParse(tenantIdClaim, out var tenantId))
            {
                var allTenants = new List<int> { 0, 1 }; // Always include global/system tenants

                var current = await dbContext.Tenants
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(t => t.Id == tenantId);

                while (current != null)
                {
                    allTenants.Add(current.Id);
                    if (current.SuperEntityId == null) break;

                    current = await dbContext.Tenants
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(t => t.Id == current.SuperEntityId);
                }

                tenantProvider.AllowedTenantIds = allTenants.Distinct().ToList();

                var toVisit = new Queue<int>(allTenants); 
                while (toVisit.Count > 0)
                {
                    var id = toVisit.Dequeue();

                    var children = await dbContext.Tenants
                        .IgnoreQueryFilters()
                        .Where(t => t.SuperEntityId == id)
                        .Select(t => t.Id)
                        .ToListAsync();

                    foreach (var childId in children)
                    {
                        if (!allTenants.Contains(childId))
                        {
                            allTenants.Add(childId);
                        }
                    }
                }

                tenantProvider.AllowedTenantIds = allTenants.ToList();
            }

            await _next(context);
        }
    }

}
