using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.TeamMemberRepo
{
    public class TeamMemberRepository : GenericRepository<TeamMembers>, ITeamMemberRepository
    {
        private readonly EntityMembershipDbContext _context;

        public TeamMemberRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<TeamMembers>> GetTeamMembersByTeamID(int teamID)
        {


            var teamMembers = await _context.TeamMembers.Where(tm => tm.TeamID == teamID).ToListAsync();

            return teamMembers;
        }

    }
}
