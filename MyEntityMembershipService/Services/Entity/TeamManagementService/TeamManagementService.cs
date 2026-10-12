using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.TeamsDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.TeamMemberRepo;
using MyEntityMembershipService.Repositories.Entity.TeamsRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;

namespace MyEntityMembershipService.Services.Entity.TeamManagementService
{
    public class TeamManagementService:ITeamManagementService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public TeamManagementService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _mapper=mapper;
            _unitOfWork=unitOfWork;
            _configuration=configuration;
        }


        public async Task<Response<bool>> CreateTeam(AddNewTeamDto teamteamDto)
        {
            try
            {
                if (teamteamDto.TeamMemberIDs == null || !teamteamDto.TeamMemberIDs.Any())
                    return new ResponseHandler().BadRequest<bool>("At least one player is required");

                var imageUrl = await DocumentSetting.AddToContainerAndGetUrl(
                    _configuration["StorageConnectionString"],
                    "buildupimagedocs",
                    teamteamDto.TeamImage
                );

                var team = new Team
                {
                    TeamName = teamteamDto.TeamName,
                    Description = teamteamDto.Description,
                    TeamImage = imageUrl,
                    SportID = teamteamDto.SportID,
                    Gender = teamteamDto.Gender,
                    CoachID = teamteamDto.CoachID, 
                    TeamMembers = teamteamDto.TeamMemberIDs.Select(memberId => new TeamMembers
                    {
                        MemberID = memberId
                    }).ToList()
                };

                await _unitOfWork.Repository<Team>().Add(team);

                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<PaginationToReturnDto<GetTeamsListDto>>> GetTeamsList(
     TeamsFilterDto? filters,
     int pageNumber,
     int pageSize)
        {
            try
            {
                var spec = new GeneralSpecifications<Team>(t =>
                    filters == null ||
                    (
                        (filters.SportList == null ||
                         !filters.SportList.Any() ||
                         filters.SportList.Contains(t.SportID))
                        &&
                        (string.IsNullOrEmpty(filters.Gender) ||
                         t.Gender.ToString() == filters.Gender)
                        &&
                        (string.IsNullOrEmpty(filters.SearchText) ||
                         t.TeamName.ToLower().Contains(filters.SearchText.ToLower()))
                    ))
                {
                    Includes =
                    [
                        t => t.Include(x => x.Sport),
                t => t.Include(x => x.TeamMembers)
                    ]
                };

                var teams = await _unitOfWork.Repository<Team>()
                    .GetAllWithSpecificationsAndProjection(
                        spec,
                        t => new GetTeamsListDto
                        {
                            TeamID = t.Id,
                            TeamName = t.TeamName,
                            ImageURL = t.TeamImage,
                            SportID = t.SportID,
                            SportName = t.Sport.SportName,
                            Gender = t.Gender.ToString(),
                            NoOfMembers = t.TeamMembers.Count,

                            // Needed to get coach name later
                            CoatchName = t.CoachID
                        });

                var totalCount = teams.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagedTeams = teams
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Get all coach ids for current page
                var coachIds = pagedTeams
                    .Where(x => !string.IsNullOrEmpty(x.CoatchName))
                    .Select(x => x.CoatchName!)
                    .Distinct()
                    .ToList();

                if (coachIds.Any())
                {
                    var coaches = await _unitOfWork
                        .Repository<Data.Membership.Entities.Membership>()
                        .FindAllByCriteriaAsync(x =>
                            coachIds.Contains(x.UserId) &&
                            x.MembershipStatus == MembershipStatus.Active);

                    var coachLookup = coaches.ToDictionary(
                        x => x.UserId,
                        x => $"{x.FirstName} {x.LastName}");

                    foreach (var team in pagedTeams)
                    {
                        if (!string.IsNullOrEmpty(team.CoatchName) &&
                            coachLookup.TryGetValue(team.CoatchName, out var coachName))
                        {
                            team.CoatchName = coachName;
                        }
                    }
                }

                var imageTasks = pagedTeams.Select(async team =>
                {
                    if (!string.IsNullOrEmpty(team.ImageURL))
                    {
                        team.ImageURL = await DocumentSetting
                            .GetPreSignedUrlAsync(team.ImageURL, 60);
                    }
                });

                await Task.WhenAll(imageTasks);

                var result = new PaginationToReturnDto<GetTeamsListDto>(
                    totalCount,
                    totalPages,
                    pageSize,
                    pageNumber,
                    pagedTeams);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler()
                    .BadRequest<PaginationToReturnDto<GetTeamsListDto>>(ex.Message);
            }
        }
        public async Task<Response<GetTeamByIdDto>> GetTeamByIdAsync(int teamId)
        {
            try
            {
                var spec = new GeneralSpecifications<Team>(t => t.Id == teamId)
                {
                    Includes =
                    [
                        t => t.Include(x => x.Sport),
                t => t.Include(x => x.TeamMembers)
                    ]
                };

                var team = (await _unitOfWork.Repository<Team>()
                    .GetAllWithSpecifications(spec))
                    .FirstOrDefault();

                if (team == null)
                {
                    return new ResponseHandler()
                        .NotFound<GetTeamByIdDto>("Team not found");
                }

                // 🔹 Get coach name from Membership
                string? coachName = null;

                if (!string.IsNullOrEmpty(team.CoachID))
                {
                    var coachMembership = await _unitOfWork
                        .Repository<Data.Membership.Entities.Membership>()
                        .FindByCriteriaAsync(m =>
                            m.UserId == team.CoachID &&
                            m.TenantId == team.GlobalTenantId &&
                            m.MembershipStatus == MembershipStatus.Active);

                    coachName = coachMembership?.FirstName+" "+coachMembership?.LastName;
                }

                var dto = new GetTeamByIdDto
                {
                    TeamID = team.Id,
                    TeamName = team.TeamName,
                    ImageURL = team.TeamImage,
                    Description=team.Description,
                    SportID = team.SportID,
                    SportName = team.Sport.SportName,
                    Gender = team.Gender.ToString(),
                    CoachName = coachName,
                    NoOfMembers = team.TeamMembers.Count
                };

                if (!string.IsNullOrEmpty(dto.ImageURL))
                {
                    dto.ImageURL = await DocumentSetting
                        .GetPreSignedUrlAsync(dto.ImageURL, 60);
                }

                return new ResponseHandler().Success(dto);
            }
            catch (Exception)
            {
                return new ResponseHandler()
                    .BadRequest<GetTeamByIdDto>("Something went wrong");
            }
        }

        public async Task<Response<List<TeamMemberInfoDto>>> GetTeamMembersByTeamIdAsync(int teamId)
        {
            try
            {
                var team = await _unitOfWork.Repository<Team>()
                    .FindByCriteriaAsync(t => t.Id == teamId);

                if (team == null)
                {
                    return new ResponseHandler()
                        .NotFound<List<TeamMemberInfoDto>>("Team not found");
                }

                // 🔹 Get team member user ids
                var teamMemberIds = await _unitOfWork
                    .Repository<TeamMembers>()
                    .FindAllByCriteriaAsync(tm => tm.TeamID == teamId);

                if (!teamMemberIds.Any())
                {
                    return new ResponseHandler()
                        .Success(new List<TeamMemberInfoDto>());
                }

                var memberUserIds = teamMemberIds
                    .Select(tm => tm.MemberID)
                    .ToList();

                var memberships = await _unitOfWork
                    .Repository<Data.Membership.Entities.Membership>()
                    .FindAllByCriteriaAsync(m =>
                        memberUserIds.Contains(m.UserId) &&
                        m.TenantId == team.GlobalTenantId &&
                        m.MembershipStatus == MembershipStatus.Active);

                var result = memberships.Select(m => new TeamMemberInfoDto
                {
                    MemberID = m.UserId,
                    Name = $"{m.FirstName} {m.LastName}",
                    UserImage = m.UserPhoto
                }).ToList();

                // 🔹 PreSigned images
                var tasks = result.Select(async member =>
                {
                    if (!string.IsNullOrEmpty(member.UserImage))
                    {
                        member.UserImage = await DocumentSetting
                            .GetPreSignedUrlAsync(member.UserImage, 60);
                    }
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception)
            {
                return new ResponseHandler()
                    .BadRequest<List<TeamMemberInfoDto>>("Something went wrong");
            }
        }
        public async Task<Response<bool>> DeleteTeamAsync(int teamId)
        {
            try
            {
                var team = await _unitOfWork.Repository<Team>().GetWithIncludesAsync(
                    teamId,
                    t => t.TeamMembers);

                if (team == null)
                {
                    return new ResponseHandler().NotFound<bool>("Team not found.");
                }

                if (team.TeamMembers != null && team.TeamMembers.Any())
                {
                    foreach (var teamMember in team.TeamMembers)
                    {
                        _unitOfWork.Repository<TeamMembers>().SoftDelete(teamMember);
                    }
                }

                _unitOfWork.Repository<Team>().SoftDelete(team);

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true, "Team and its members successfully deleted.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }






    }
}
