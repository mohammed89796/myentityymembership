using MyEntityMembershipService.DTOs.Entity.DashboardDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.DashboardServ
{
    public interface IDashboardServ
    {
        Task<Response<DashboardTenentDetails>> GetDashboardTenatSummary(int? tenantId);
    }
}
