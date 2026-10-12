using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.Repositories.GenericRepo;
using System.Linq.Expressions;

namespace MyEntityMembershipService.APi.Repositories.Series_Repo
{
    public class SeriesRepo : GenericRepository<Series>, ISeriesRepo
    {
        private readonly EntityMembershipDbContext _context;

        public SeriesRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Series>> GetAllWithEventAndEventDetailAsync(Expression<Func<Series, bool>> criteria)
        {
            if (criteria is not null)
                return await _context.Set<Series>().Where(criteria).Include(s => s.Events).ThenInclude(e => e.EventDetails).ToListAsync();

            return await _context.Set<Series>().Include(s => s.Events).ThenInclude(e => e.EventDetails).ToListAsync();
        }



        public async Task<Series> GetByIdWithEventAndEventDetailAsync(int id)

            => await _context.Set<Series>().Include(s => s.Events).ThenInclude(e => e.EventDetails).FirstOrDefaultAsync(s => s.Id == id);



        public async Task<IReadOnlyList<Series>> FilterSeriesAsync(Expression<Func<Series, bool>> criteria)


            => await _context.Set<Series>().Where(criteria).Include(s => s.Events).ThenInclude(e => e.EventDetails).ToListAsync();


        public async Task<IReadOnlyList<Series>> SortSeriesAsync(Expression<Func<Series, object>> criteria)


           => await _context.Set<Series>().OrderBy(criteria).Include(s => s.Events).ThenInclude(e => e.EventDetails).ToListAsync();


        public async Task<IReadOnlyList<Series>> SortSeriesDescendingAsync(Expression<Func<Series, object>> criteria)


           => await _context.Set<Series>().OrderByDescending(criteria).Include(s => s.Events).ThenInclude(e => e.EventDetails).ToListAsync();
    }
}
