
using MyEntityMembershipService.DTOs.Entity.SystemDtos.FedrationBranches;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.FedrationBranchesRepo
{
    public interface IFedrationBranchesRepo : IGenericRepository<FedrationBranch>
    {
        public Task<List<FedrationBranchesViewModel>> GetFedBranchesDropDown(int tenantId, int languageType);

    }
}
