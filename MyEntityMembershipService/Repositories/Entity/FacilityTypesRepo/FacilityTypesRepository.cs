using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.FacilityTypesRepo
{
    public class FacilityTypesRepository : GenericRepository<FacilityType>, IFacilityTypesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public FacilityTypesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<FacilityType>> GetAllFacilityTypesAsync()
        {
            return await _context.FacilityType.ToListAsync();
        }
    }
}
