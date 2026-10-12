using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.CancellationConfigurationRepository
{
    public class CancellationConfigurationRepository : GenericRepository<CancellationConfiguration>, ICancellationConfigurationRepository
    {
        private readonly EntityMembershipDbContext _context;

        public CancellationConfigurationRepository(EntityMembershipDbContext context) : base(context)
            =>  _context = context;
        public async Task<CancellationConfiguration> GetWithUnitAsync(int packageId)
            => await _context.Set<CancellationConfiguration>().Where(config => config.PackageId == packageId)
            .Include(config => config.Unit)
            .FirstOrDefaultAsync();
    }
}
