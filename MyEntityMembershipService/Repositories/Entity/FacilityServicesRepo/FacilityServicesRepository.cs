using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;



using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.FacilityServicesRepo
{
    public class FacilityServicesRepository : GenericRepository<FacilityServices>, IFacilityServicesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public FacilityServicesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<CoreFacilityServices>> GetAllFacilityServicesAsync()
        {
            return await _context.CoreFacilityServices.ToListAsync();
        }
    }
}
