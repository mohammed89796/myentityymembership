using MyEntityMembershipService.Data;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.BranchMediaRepo
{
    public class BranchMediaRepository : GenericRepository<BranchImages>, IBranchMediaRepository
    {
        private readonly EntityMembershipDbContext _context;

        public BranchMediaRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
