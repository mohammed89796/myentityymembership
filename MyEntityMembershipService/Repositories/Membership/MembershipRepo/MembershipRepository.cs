using MyEntityMembershipService.Data;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.MembershipRepo
{
    public class MembershipRepository : GenericRepository<Data.Membership.Entities.Membership>, IMembershipRepository
    {
        private readonly EntityMembershipDbContext context;

        public MembershipRepository(EntityMembershipDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Data.Membership.Entities.Membership> GetMembershipWithSubMembers(int Id)
            => await context.Memberships.Where(membership => membership.Id == Id).Include(membership => membership.SubMembers).FirstOrDefaultAsync();

        public async Task<List<Data.Membership.Entities.Membership>> GetUserMemberships(string userId)
            => await context.Memberships.Where(membership => membership.UserId == userId && !membership.ParentMembershipId.HasValue).OrderByDescending(m => m.CreationDate).ToListAsync();

        public async Task<List<Data.Membership.Entities.Membership>> GetUserMembershipsByName(string userId, string name)
            => await context.Memberships.Where(membership => membership.FirstName.Contains(name.Trim().ToLower()) && membership.UserId == userId && !membership.ParentMembershipId.HasValue).OrderByDescending(m => m.CreationDate).ToListAsync();
        public async Task<List<Data.Membership.Entities.Membership>> GeMembershipByPackageId(int Id)
          => await context.Memberships.Where(membership => membership.PackageId == Id).OrderByDescending(m => m.CreationDate).ToListAsync();

        public async Task<Data.Membership.Entities.Membership> GetUserMembershipByPackageId(string userId, int PackageId)
            => await context.Memberships.FirstOrDefaultAsync(membership => membership.UserId == userId && membership.PackageId == PackageId);
        public async Task<List<Data.Membership.Entities.Membership>> GeMembershipByTenantId(int tenantId)
                => await context.Memberships.Where(membership => membership.TenantId == 1 ||  membership.TenantId == tenantId).ToListAsync();
    }
}
