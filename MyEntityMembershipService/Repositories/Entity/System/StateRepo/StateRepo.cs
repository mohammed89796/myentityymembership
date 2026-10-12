using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.SystemDtos.StateDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.StateRepo
{
    public class StateRepo : GenericRepository<State>, IStateRepo
    {
        public DbSet<State> dbSet;
        private readonly EntityMembershipDbContext _context;

        public StateRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
            dbSet = context.Set<State>();

        }

        public async Task<List<StateViewModel>> GetStates(int countryID)
        {

            IQueryable<State> query;

            query = dbSet.Where(s => s.CountryID == countryID).OrderBy(t => t.CreationDate);

            var response = await query
                .Select(
                    a =>
                        new StateViewModel
                        {
                            ID = a.Id,
                            StateName = a.Name
                        }
                )
                .ToListAsync();

            return response;
        }
    }
}
