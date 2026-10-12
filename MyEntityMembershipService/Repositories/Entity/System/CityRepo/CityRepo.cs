using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.SystemDtos.CityRepo;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.CityRepo
{
    public class CityRepo : GenericRepository<City>, ICityRepo
    {
        public DbSet<City> dbSet;
        private readonly EntityMembershipDbContext _context;

        public CityRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
            dbSet = context.Set<City>();

        }

        public async Task<List<CityViewModel>> GetCities(int stateID)
        {

            IQueryable<City> query;

            query = dbSet.Where(s => s.StateID == stateID).OrderBy(t => t.CreationDate);

            var response = await query
                .Select(
                    a =>
                        new CityViewModel
                        {
                            ID = a.Id,
                            CityName = a.Name
                        }
                )
                .ToListAsync();

            return response;
        }
    }
}
