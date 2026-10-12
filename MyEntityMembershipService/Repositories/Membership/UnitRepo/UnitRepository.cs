using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.UnitRepo
{
    public class UnitRepository : GenericRepository<Unit>, IUnitRepository
    {
        private readonly EntityMembershipDbContext _context;

        public UnitRepository(EntityMembershipDbContext context) : base(context)
            => _context = context;
        
        public async Task<List<Unit>> GetAllWithUnitTypeAsync()
            => await _context.Set<Unit>().Include(unit => unit.UnitType).ToListAsync();

        public async Task<Unit> GetByIdWithUnitTypeAsync(int id)
            => await _context.Set<Unit>()
                            .Where(unit => unit.Id == id)
                            .Include(unit => unit.UnitType)
                            .FirstOrDefaultAsync();

        public async Task<Unit> GetByNameWithUnitTypeAsync(string name)
            => await _context.Set<Unit>()
                            .Where(unit => unit.Name.Contains(name))
                            .Include(unit => unit.UnitType)
                            .FirstOrDefaultAsync();

        public async Task<List<Unit>> GetByUnitTypeIdWithTypeNameAsync(int unitTypeId)
            => await _context.Set<Unit>()
            .Where(unit => unit.UnitTypeId == unitTypeId)
            .Include(unit => unit.UnitType)
            .ToListAsync();
    }
}
