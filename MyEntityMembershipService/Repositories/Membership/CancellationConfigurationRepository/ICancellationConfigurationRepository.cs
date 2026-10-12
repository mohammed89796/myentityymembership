using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.CancellationConfigurationRepository
{
    public interface ICancellationConfigurationRepository : IGenericRepository<CancellationConfiguration>
    {
        Task<CancellationConfiguration> GetWithUnitAsync(int packageId);
    }
}
