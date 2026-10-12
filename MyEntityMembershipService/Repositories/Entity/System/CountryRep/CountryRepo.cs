using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.SystemDtos.CountryDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.CountryRep
{
    public class CountryRepo : GenericRepository<Country>, ICountryRepo
    {
        public DbSet<Country> dbSet;
        private readonly EntityMembershipDbContext _context;

        public CountryRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
            dbSet = context.Set<Country>();
        }

        public async Task<List<CountryViewModel>> GetCountries()
        {

            IQueryable<Country> query;

            query = dbSet.OrderBy(t => t.CreationDate);

            var response = await query
                .Select(
                    a =>
                        new CountryViewModel
                        {
                            ID = a.Id,
                            CountryName = a.Name
                        }
                )
                .ToListAsync();

            return response;
        }
    }
}
