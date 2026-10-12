using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;



using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.SportsRepo
{
    public class SportsRepository : GenericRepository<Sports>, ISportsRepository
    {
        private readonly EntityMembershipDbContext _context;

        public SportsRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Sports>> GetAllSportsAsync()
        {
            return await _context.Sports.ToListAsync();
        }
    }
}
