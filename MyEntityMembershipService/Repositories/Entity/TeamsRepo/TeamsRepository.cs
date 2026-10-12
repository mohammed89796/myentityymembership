using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;



using MyEntityMembershipService.DTOs.Entity.TeamsDtos;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.TeamsRepo
{
    public class TeamsRepository : GenericRepository<Team>, ITeamsRepository
    {
        private readonly EntityMembershipDbContext _context;

        public TeamsRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<GetTeamsListDto>> GetTeamsListAsync(TeamsFilterDto? filters)
        {
            var query = _context.Teams
                .Join(
                    _context.Sports,
                    team => team.SportID,
                    sport => sport.Id,
                    (team, sport) => new GetTeamsListDto
                    {
                        TeamID = team.Id,
                        TeamName = team.TeamName,
                        ImageURL = team.TeamImage,
                        SportID = team.SportID,
                        SportName = sport.SportName,
                        Gender = team.Gender == 1 ? "Male" : team.Gender == 2 ? "Female" : "Unknown"
                    }
                );

            if (filters.SportList != null && filters.SportList.Any())
            {
                query = query.Where(t => filters.SportList.Contains(t.SportID));
            }

            if (!string.IsNullOrEmpty(filters.Gender))
            {
                query = query.Where(t => t.Gender == filters.Gender);
            }
            if (!string.IsNullOrEmpty(filters.SearchText))
            {
                string searchText = filters.SearchText.ToLower();
                query = query.Where(t => t.TeamName.ToLower().Contains(searchText));
            }

            if (!string.IsNullOrEmpty(filters.SortField))
            {
                query = filters.SortField.ToLower() switch
                {
                    "teamname" => query.OrderBy(t => t.TeamName),
                    "sport" => query.OrderBy(t => t.SportName),
                    "gender" => query.OrderBy(t => t.Gender),
                    _ => query.OrderBy(t => t.TeamID)
                };
            }

            return await query.ToListAsync();
        }

        public async Task<TeamInfoDto> GetTeamDetailsAsync(int teamId)
        {
            var teamDetails = await _context.Teams
                .Where(team => team.Id == teamId)
                .Select(team => new TeamInfoDto
                {
                    TeamImage = team.TeamImage,
                    TeamName = team.TeamName,
                    CoachName = "Mo7sen",
                    Gender = team.Gender == 1 ? "Male" : team.Gender == 2 ? "Female" : "Unknown",
                    Description = team.Description
                })
                .FirstOrDefaultAsync();

            return teamDetails;
        }
        public async Task<PaginatedTeamMemberInfoDto> GetTeamMembersAsync(int teamId, int pageNumber)
        {
            int pageSize = 10;
            var query = _context.TeamMembers
                .Where(member => member.TeamID == teamId)
                .Select(member => new TeamMemberInfoDto
                {
                    MemberID = null,
                    UserImage = null,
                    Name = "Mo7sen Jr."
                });

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedTeamMembers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedTeamMemberInfoDto
            {
                TotalPages = totalPages,
                TeamMembers = paginatedTeamMembers
            };
        }


        public async Task<GetEditTeamDto> GetEditTeamDetails(int teamId)
        {
            var teamDetails = await _context.Teams
                .Where(t => t.Id == teamId)
                .Select(t => new GetEditTeamDto
                {
                    TeamID = t.Id,
                    TeamImage = t.TeamImage,
                    TeamName = t.TeamName,
                    Gender = t.Gender,
                    SportID = t.SportID,
                    Description = t.Description,
                    CoachID = t.CoachID,
                    TeamMemberIDs = t.TeamMembers.Select(tm => tm.MemberID).ToList()
                })
                .FirstOrDefaultAsync();

            return teamDetails;
        }



    }
}
