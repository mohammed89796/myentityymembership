using MyEntityMembershipService.DTOs.Entity.SystemDtos.FedrationBranches;

using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.System.EntityBranchService
{
    public interface IFedrationBranchService
    {
        public Task<Response<bool>> CreateFedrationBranch(string enName, string arName, int TenantId);
        public Task<Response<List<FedrationBranchesViewModel>>> GetBranchDropDown(int tenantId, int languageType);


    }
}