using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.TurfTypesRepo
{
    public class TurfTypesRepository : GenericRepository<TurfTypes>, ITurfTypesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public TurfTypesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<TurfTypes>> GetAllTurfTypesAsync()
        {
            return await _context.TurfTypes.ToListAsync();
        }
    }
}
