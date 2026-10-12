using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.PackageRepo
{
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        private readonly EntityMembershipDbContext _context;

        public PackageRepository(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<Package>> GetAllPackageByTenantId(int? tenantId)
        {
            return await _context.Packages
                .Where(p => p.PackageType == PackageType.Public
                         && p.IsActive == true
                         && (!tenantId.HasValue || p.TenantId == tenantId.Value))
                .Include(b => b.SubMemberCategories)
                .Include(p => p.Facilities)
                .Include(p => p.Branches)
                .Include(p => p.Venues)
                .Include(p => p.Tenant)
                .ToListAsync();
        }


        public async Task<Package> GetPackageById(int id)
        {
            var package = await _context.Packages
                .Include(p => p.SubMemberCategories)
                .Include(p => p.Branches)
                .ThenInclude(ab => ab.Branch)

                    .ThenInclude(b => b.AccessedBranchesFacilities
                        .Where(abf => abf.PackageId == id))
                        .ThenInclude(abf => abf.Facility)
                        .ThenInclude(abf => abf.FacilityType)

                .Include(p => p.Branches)
                .ThenInclude(ab => ab.Branch)
                    .ThenInclude(b => b.AccessedBranchesFacilities
                        .Where(abf => abf.PackageId == id))
                        .ThenInclude(abf => abf.Facility)
                        .ThenInclude(abf => abf.AccessedFacilitiesVenues
                            .Where(afv => afv.PackageId == id))
                            .ThenInclude(afv => afv.Venue)
                .FirstOrDefaultAsync(t => t.Id == id);

            return package;
        }


        public async Task<IReadOnlyList<Package>> GetPackageByTenantId(int tenantId)
        {
            return await _context.Packages
                .Include(b => b.SubMemberCategories)

                .Include(b => b.Branches)
                .Include(p => p.Facilities)
                .Include(p => p.Venues)
                .Where(p => !p.IsDeleted && p.TenantId == tenantId && p.IsActive == true) // Exclude deleted packages
                .ToListAsync();
        }

        public async Task<IReadOnlyList<GetPackageDto>> GetPackagesByTenantIdAndByListOfIds(int tenantId, List<int> packageIds)
            => await _context.Packages
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.TenantId == tenantId && packageIds.Contains(p.Id))
                .Select(p => new GetPackageDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.Image,
                    Duration = p.Duration,
                    Price = p.Price,
                    TenantId = p.TenantId,
                    BranchesCount = p.Branches.Count,
                    FacilitiesCount = p.Facilities.Count,
                    VenuesCount = p.Venues.Count
                })
                .ToListAsync();



        public async Task<IReadOnlyList<GetPackageDto>> GetAllPackagesByTenantId(int tenantId)
           => await _context.Packages
               .AsNoTracking()
               .Where(p => !p.IsDeleted && p.TenantId == tenantId)
                .Select(p => new GetPackageDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.Image,
                    Duration = p.Duration,
                    Price = p.Price,
                    TenantId = p.TenantId,
                    BranchesCount = p.Branches.Count,
                    FacilitiesCount = p.Facilities.Count,
                    VenuesCount = p.Venues.Count
                }).ToListAsync();


        public async Task SoftDeletePackage(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package != null)
            {
                package.IsDeleted = true;
                _context.Packages.Update(package);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Package> GetPackageWithTenant(int id)
            => await _context.Packages.Where(p => p.Id == id)
                                      .Include(p => p.Tenant)
                                      .FirstOrDefaultAsync();
    }
}
