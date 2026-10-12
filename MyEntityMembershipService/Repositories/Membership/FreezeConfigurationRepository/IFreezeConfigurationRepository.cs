using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.FreezeConfigurationRepository
{
    public interface IFreezeConfigurationRepository : IGenericRepository<FreezeConfiguration>
    {
        Task<FreezeConfiguration> GetByIdWithUnitAsync(int packageId);
    }
}
