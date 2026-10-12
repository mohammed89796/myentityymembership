using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.EventDetailRepo
{
    public class EventDetailRepo : GenericRepository<EventDetail>, IEventDetailRepo
    {
        private readonly EntityMembershipDbContext _context;

        public EventDetailRepo(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<EventDetail>> GetAllWithEventAsync()

           => await _context.Set<EventDetail>().Include(e => e.Event).ToListAsync();


        public async Task<EventDetail> GetByIdWithEventAsync(int id)

            => await _context.Set<EventDetail>().Include(e => e.Event).FirstOrDefaultAsync(e => e.Id == id);
    }
}
