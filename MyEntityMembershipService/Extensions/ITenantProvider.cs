using MyEntityMembershipService.Data;
using MyEntityMembershipService.Migrations;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Entity.TenantService;
using System.Security.Claims;

namespace MyEntityMembershipService.Extensions
{
    public interface ITenantProvider
    {
        int? GlobalTenantId { get;}    
        bool IgnoreTenantFilter { get; set; }
        List<int> AllowedTenantIds { get; set; }
        void SetTenantContext(int tenantId);
    }


    public class TenantProvider : ITenantProvider
    {
        public List<int> AllowedTenantIds { get; set; } = new();
        private int? _globalTenantId;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenantProvider(IServiceScopeFactory scopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            _scopeFactory = scopeFactory;
            _httpContextAccessor=httpContextAccessor;
        }

        public bool IgnoreTenantFilter { get; set; } = false;

        public int? GlobalTenantId
        {
            get
            {
                if (_globalTenantId.HasValue)
                    return _globalTenantId;

                var claimValue = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("tenantId")?.Value;

                if (int.TryParse(claimValue, out var parsedId))
                {
                    _globalTenantId = parsedId;
                }

                return _globalTenantId;
            }
        }
        public void SetTenantContext(int tenantId)
        {
            _globalTenantId = tenantId;
        }
    }

}
