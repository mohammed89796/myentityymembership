using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.TenantRepo
{
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        private readonly EntityMembershipDbContext _context;

        public TenantRepository(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tenant>> GetAllTenantsAsync(int typeId)
        {
            return await _context.Tenants
                .Include(t => t.Packages)
                .Include(t => t.Branches)
                    .ThenInclude(tb => tb.NewFacilities)
                        .ThenInclude(tbv => tbv.NewVenues)
                            .ThenInclude(vo => vo.OperatingHours)
                            .Where(t => t.EntityTypeId == typeId && t.SuperEntityId==null)

                .ToListAsync();
        }

        public async Task<Tenant> GetTenantByIdAsync(int id, int typeId)
        {
            return await _context.Tenants
                .Include(t => t.Packages)
                .Include(t => t.Branches)
                    .ThenInclude(tb => tb.NewFacilities)
                        .ThenInclude(tbv => tbv.NewVenues)
                            .ThenInclude(vo => vo.OperatingHours)
                                .Where(t => t.EntityTypeId == typeId)

                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IReadOnlyList<Tenant>> GetTenantsByListOfIdsAsync(List<int> tenantIds)

            => await _context.Tenants.Where(v => tenantIds.Contains(v.Id) && v.EntityTypeId == 1).ToListAsync();


        public async Task<Tenant> GeFedByIdAsync(int id, int typeId)
        {
            return await _context.Tenants
                .Include(t => t.Packages)
                .Include(t => t.Branches)
                    .ThenInclude(tb => tb.NewFacilities)
                        .ThenInclude(tbv => tbv.NewVenues)
                            .ThenInclude(vo => vo.OperatingHours)
                                .Where(t => t.EntityTypeId == typeId)

                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<IEnumerable<Tenant>> GetAllFedrationsAsync(int typeId)
        {
            return await _context.Tenants
                .Include(t => t.Packages)
                .Include(t => t.Branches)
                    .ThenInclude(tb => tb.NewFacilities)
                        .ThenInclude(tbv => tbv.NewVenues)
                            .ThenInclude(vo => vo.OperatingHours)
                            .Where(t => t.Id == 2)

                .ToListAsync();
        }
        public async Task<string> CheckEntityType(int Id)
        {
            return await _context.Tenants.Where(t => t.Id == Id).Select(t => t.EntityType.Name).FirstOrDefaultAsync();

        }




    }
}
