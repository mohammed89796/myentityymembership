using MyEntityMembershipService.DTOs.Entity.TenantDtos;

namespace MyEntityMembershipService.DTOs.Entity.DashboardDtos
{
    public class DashboardTenentDetails
    {
        public AddTenantDto Tenant { get; set; }
        public int TotalNoumberOfFacilities { get; set; }
        public int TotalNoumberOfMemberships { get; set; }
        public int TotalNoumberOfPrograms { get; set; }


    }
}
