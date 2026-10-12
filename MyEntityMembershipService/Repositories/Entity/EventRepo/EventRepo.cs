using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.Repositories.GenericRepo;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Repositories.Entity.EventRepo
{
    public class EventRepo : GenericRepository<Event>, IEventRepo
    {
        private readonly EntityMembershipDbContext _context;

        public EventRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<List<Event>> GetAllWithEventDetailAsync()

            => await _context.Set<Event>().Include(e => e.Series).Include(e => e.EventDetails).ToListAsync();

        public async Task<List<Event>> GetAllWithEventDetailAsync(int seriesId, int pageNumber, int pageSize)

            => await _context.Set<Event>()
            .Where(e => e.SeriesId == seriesId)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Include(e => e.Series)
            .Include(e => e.EventDetails)
            .ToListAsync();


        public async Task<int> GetCountOfEventsAsync(int seriesId)

             => await _context.Set<Event>().CountAsync(e => e.SeriesId == seriesId);



        public async Task<Event> GetByIdWithEventDetailAsync(int id)

            => await _context.Set<Event>().Include(e => e.Series).Include(e => e.EventDetails).FirstOrDefaultAsync(e => e.Id == id);


        public async Task<int> GetSumOfPriceOfAllEventsAsync(int seriesId)

            => await _context.Set<Event>().Where(e => e.SeriesId == seriesId).SumAsync(e => e.Price ?? 0);


        public async Task<List<Event>> FilterEventsAsync(Expression<Func<Event, bool>> criteria, int pageNumber, int pageSize)

            => await _context.Set<Event>()
            .Where(criteria)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Include(e => e.Series)
            .Include(e => e.EventDetails)
            .ToListAsync();

        public async Task<int> GetCountOfFilteredEventsAsync(Expression<Func<Event, bool>> criteria)

            => await _context.Set<Event>()
            .CountAsync(criteria);

        public async Task<List<Event>> SortEventsAsync(int seriesId, Expression<Func<Event, object>> criteria, int pageNumber, int pageSize)

            => await _context.Set<Event>()
            .Where(e => e.SeriesId == seriesId)
            .OrderBy(criteria)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Include(e => e.Series)
            .Include(e => e.EventDetails)
            .ToListAsync();

        public async Task<List<Event>> SortEventsDescAsync(int seriesId, Expression<Func<Event, object>> criteria, int pageNumber, int pageSize)

            => await _context.Set<Event>()
            .Where(e => e.SeriesId == seriesId)
            .OrderByDescending(criteria)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Include(e => e.Series)
            .Include(e => e.EventDetails)
            .ToListAsync();

    }
}
