using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.AmenitiesRepo
{
    public class AmenitiesRepository : GenericRepository<Amenities>, IAmenitiesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public AmenitiesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Amenities>> GetAllAmenitiesAsync()
        {
            return await _context.Amenities.ToListAsync();
        }
    }
}
