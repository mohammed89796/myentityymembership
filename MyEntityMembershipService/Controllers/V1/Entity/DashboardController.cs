using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.DashboardServ;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [ApiController]
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [Authorize]

    public class DashboardController : ControllerBase
    {
        private readonly IDashboardServ dashboardServ;

        public DashboardController(IDashboardServ dashboardServ)
        {
            this.dashboardServ = dashboardServ;
        }

        [HttpGet("GetDashboardTenatSummary")]
        public async Task<IResult> GetDashboardTenatSummary()
        {
            var tenantId = User.GetParsedTenantId();
            var result = await dashboardServ.GetDashboardTenatSummary(tenantId);
            return result.GetResponse();
        }

    }
}
