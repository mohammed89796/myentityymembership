using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.FreezeConfigurationRepository
{
    public class FreezeConfigurationRepository : GenericRepository<FreezeConfiguration>, IFreezeConfigurationRepository
    {
        private readonly EntityMembershipDbContext _context;

        public FreezeConfigurationRepository(EntityMembershipDbContext context) : base(context)
            =>  _context = context;

        public async Task<FreezeConfiguration> GetByIdWithUnitAsync(int packageId)
            => await _context.Set<FreezeConfiguration>()
            .Where(config => config.PackageId == packageId)
            .Include(config => config.Unit)
            .FirstOrDefaultAsync();
    }
}
