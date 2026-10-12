using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.CountryCodeRepo
{
    public class CountryCodesRepository : GenericRepository<CountryCodes>, ICountryCodesRepository
    {
        private readonly EntityMembershipDbContext _context;

        public CountryCodesRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<CountryCodes>> GetAllCountryCodesAsync()
        {
            return await _context.CountryCodes.ToListAsync();
        }
    }
}
