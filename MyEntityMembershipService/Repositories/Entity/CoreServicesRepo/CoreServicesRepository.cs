using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;



using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.CoreServicesRepo
{
    public class CoreServicesRepository : GenericRepository<CoreService>, ICoreServicesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public CoreServicesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<CoreService>> GetAllCoreServicesAsync()
        {
            return await _context.CoreServices.ToListAsync();
        }
    }
}
