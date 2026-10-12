using Microsoft.EntityFrameworkCore;

using MyEntityMembershipService.Data;



using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.BranchServicesRepo
{
    public class BranchServicesRepository : GenericRepository<BranchServices>, IBranchServicesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public BranchServicesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<List<BranchServices>> GetBranchServicesByBranchIdAsync(int branchId)
        {
            return await _context.BranchServices
                .Where(service => service.BranchID == branchId)
                .ToListAsync() ?? new List<BranchServices>();
        }

    }
}
