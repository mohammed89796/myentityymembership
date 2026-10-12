using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.RemovalConfigurationRepository
{
    public interface IRemovalConfigurationRepository : IGenericRepository<RemovalConfiguration>
    {
        //Task<RemovalConfiguration> GetByIdWithUnitAsync(int packageId);
        Task<RemovalConfiguration> GetByPackageIdWithSubMembersAsync(int packageId);
        Task<double?> GetRemovalFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null);
    }
}
