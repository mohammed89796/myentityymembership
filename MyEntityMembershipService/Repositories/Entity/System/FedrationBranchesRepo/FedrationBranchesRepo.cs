using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.SystemDtos.FedrationBranches;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.FedrationBranchesRepo
{
    public class FedrationBranchesRepo : GenericRepository<FedrationBranch>, IFedrationBranchesRepo
    {
        public DbSet<FedrationBranch> dbSet;
        private readonly EntityMembershipDbContext _context;

        public FedrationBranchesRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
            dbSet = context.Set<FedrationBranch>();

        }

        public async Task<List<FedrationBranchesViewModel>> GetFedBranchesDropDown(int tenantId, int languageType)
        {
            var query = dbSet.Where(fb => fb.TenantId == tenantId);

            var response = await query
                .Select(fb => new FedrationBranchesViewModel
                {
                    ID = fb.Id, 
                    Name = languageType == 1 ? fb.Name : fb.ArabicName
                })
                .ToListAsync();

            return response;
        }

    }
}
