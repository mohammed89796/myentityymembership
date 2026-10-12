using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.PollRepo
{
    public class PollRepository : GenericRepository<Poll>, IPollRepository
    {
        private readonly EntityMembershipDbContext _context;

        public PollRepository(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Poll> GetPollWithRelatedDataByIdAsync(int pollId, string userId)
            => await _context.Polls
            .Where(p => p.Id == pollId)
            .Include(p => p.Choices)
            .ThenInclude(p => p.UserChoices.Where(uc => uc.CreatedBy == userId))
            .FirstOrDefaultAsync();

    }
}
