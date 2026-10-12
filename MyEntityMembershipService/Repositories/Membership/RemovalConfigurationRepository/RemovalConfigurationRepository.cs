using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.RemovalConfigurationRepository
{
    public class RemovalConfigurationRepository : GenericRepository<RemovalConfiguration>, IRemovalConfigurationRepository
    {
        private readonly EntityMembershipDbContext _context;

        public RemovalConfigurationRepository(EntityMembershipDbContext context) : base(context)
            => _context = context;

        public async Task<RemovalConfiguration> GetByPackageIdWithSubMembersAsync(int packageId)
            => await _context.Set<RemovalConfiguration>()
            .Where(config => config.PackageId == packageId)
            .Include(config => config.SubMemberRemovalConfigurations)
            .ThenInclude(subMember => subMember.MemberType)
            .FirstOrDefaultAsync();
        
        public async Task<double?> GetRemovalFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null)
        {
           var entity = await _context.Set<RemovalConfiguration>()
            .Where(config => config.PackageId == packageId)
            .Include(config => config.SubMemberRemovalConfigurations)
            .FirstOrDefaultAsync();

            var removalFee = entity.SubMemberRemovalConfigurations.Where(subMember => subMember.SubMemberId == subMemberId)
                .Select(subMember => subMember.MembersRemovalFee)
                .FirstOrDefault();

            return removalFee;
        }
             
       
        //public async Task<RemovalConfiguration> GetByIdWithUnitAsync(int packageId)
        //    => await _context.Set<RemovalConfiguration>()
        //        .Where(config => config.PackageId == packageId)
        //        .Include(config => config.Unit)
        //        .SingleOrDefaultAsync();
    }
}
