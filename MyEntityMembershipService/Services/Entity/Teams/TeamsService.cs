using AutoMapper;
using MyEntityMembershipService.DTOs.Entity.TeamsDtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.TeamMemberRepo;
using MyEntityMembershipService.Repositories.Entity.TeamsRepo;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;


namespace MyEntityMembershipService.Services.Entity.Teams
{
    public class TeamsService : ITeamsService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ITeamsRepository _teamsRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;


        public TeamsService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ITeamsRepository teamsRepository, ITeamMemberRepository teamMemberRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _teamsRepository = teamsRepository;
            _teamMemberRepository = teamMemberRepository;
        }

        public async Task<Response<bool>> CreateNewTeam(AddNewTeamDto request)
        {
            try
            {
                if (request is null)
                {
                    return new ResponseHandler().NotFound<bool>("Failed to create new team");
                }

                // Validate required fields
                if (request.TeamImage is null)
                {
                    return new ResponseHandler().BadRequest<bool>("Team image is required");
                }

                if (string.IsNullOrEmpty(request.Description) && string.IsNullOrEmpty(request.TeamName))
                {
                    return new ResponseHandler().BadRequest<bool>("Team description and name0 are required");
                }

                if (request.SportID <= 0)
                {
                    return new ResponseHandler().BadRequest<bool>("Invalid sport selection");
                }

                if (request.TeamMemberIDs == null || request.TeamMemberIDs.Count == 0)
                {
                    return new ResponseHandler().BadRequest<bool>("At least one player must be assigned to the team");
                }

                var imageUrl = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", request.TeamImage);

                var team = new Team
                {
                    TeamName = request.TeamName,
                    TeamImage = imageUrl,
                    Gender = request.Gender,
                    SportID = request.SportID,
                    Description = request.Description,
                    CoachID = request.CoachID
                };

                await _unitOfWork.Repository<Team>().Add(team);
                await _unitOfWork.Save();

                var teamID = team.Id;

                var teamMembers = request.TeamMemberIDs.Select(memberID => new TeamMembers
                {
                    TeamID = teamID,
                    MemberID = memberID
                }).ToList();

                if (teamMembers.Any())
                {
                    await _unitOfWork.Repository<TeamMembers>().AddRange((ICollection<TeamMembers>)teamMembers);
                    await _unitOfWork.Save();
                }

                return new ResponseHandler().Created(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<List<GetTeamsListDto>>> GetTeamsList(TeamsFilterDto? filters)
        {
            try
            {

                var teamsList = await _teamsRepository.GetTeamsListAsync(filters);

                if (teamsList is null)
                {
                    return new ResponseHandler().NotFound<List<GetTeamsListDto>>("Teams list not found");
                }

                var tasks = teamsList.Select(async team =>
                {
                    if (!string.IsNullOrEmpty(team.ImageURL))
                    {
                        team.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(team.ImageURL, 60);
                    }
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(teamsList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<GetTeamsListDto>>("Something went wrong");
            }
        }

        public async Task<Response<TeamInfoDto>> GetTeamBasicDetails(int teamID)
        {
            try
            {
                var teamInfo = await _teamsRepository.GetTeamDetailsAsync(teamID);

                if (teamInfo is null)
                {
                    return new ResponseHandler().NotFound<TeamInfoDto>("Team details not found");
                }

                if (!string.IsNullOrEmpty(teamInfo.TeamImage))
                {
                    teamInfo.TeamImage = await DocumentSetting.GetPreSignedUrlAsync(teamInfo.TeamImage, 60);
                }

                return new ResponseHandler().Success(teamInfo);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<TeamInfoDto>("Something went wrong");
            }
        }
        public async Task<Response<PaginatedTeamMemberInfoDto>> GetTeamMembers(int teamID, int pageNumber)
        {
            try
            {
                var teamMembers = await _teamsRepository.GetTeamMembersAsync(teamID, pageNumber);

                if (teamMembers is null)
                {
                    return new ResponseHandler().NotFound<PaginatedTeamMemberInfoDto>("Team members not found");
                }

                var tasks = teamMembers.TeamMembers.Select(async member =>
                {
                    if (!string.IsNullOrEmpty(member.UserImage))
                    {
                        member.UserImage = await DocumentSetting.GetPreSignedUrlAsync(member.UserImage, 60);
                    }
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(teamMembers);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginatedTeamMemberInfoDto>("Something went wrong");
            }
        }
        public async Task<Response<GetEditTeamDto>> GetEditTeamDetails(int teamID)
        {
            try
            {
                var teamDetails = await _teamsRepository.GetEditTeamDetails(teamID);

                if (teamDetails is null)
                {
                    return new ResponseHandler().NotFound<GetEditTeamDto>("Team details not found");
                }

                teamDetails.TeamImage = await DocumentSetting.GetPreSignedUrlAsync(teamDetails.TeamImage, 60);

                return new ResponseHandler().Success(teamDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetEditTeamDto>("Something went wrong");
            }
        }
        public async Task<Response<bool>> EditTeam(EditTeamDto request)
        {
            try
            {
                if (request == null)
                {
                    return new ResponseHandler().BadRequest<bool>("Invalid data provided for team update.");
                }

                var existingTeam = await _unitOfWork.Repository<Team>().GetByIdAsync(request.TeamID);

                if (existingTeam == null)
                {
                    return new ResponseHandler().NotFound<bool>("Team not found.");
                }

                if (!string.IsNullOrEmpty(request.TeamName))
                {
                    existingTeam.TeamName = request.TeamName;
                }

                if (request.Gender.HasValue)
                {
                    existingTeam.Gender = request.Gender.Value;
                }

                if (request.SportID.HasValue && request.SportID.Value > 0)
                {
                    existingTeam.SportID = request.SportID.Value;
                }

                if (!string.IsNullOrEmpty(request.Description))
                {
                    existingTeam.Description = request.Description;
                }

                if (request.CoachID is not null)
                {
                    existingTeam.CoachID = request.CoachID;
                }

                if (request.TeamImage != null)
                {
                    existingTeam.TeamImage = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", request.TeamImage);
                }

                if (request.TeamMemberIDs != null)
                {
                    var existingMembers = await _teamMemberRepository.GetTeamMembersByTeamID(request.TeamID);

                    var membersToRemove = existingMembers.Where(em => !request.TeamMemberIDs.Contains(em.MemberID)).ToList();
                    if (membersToRemove.Any())
                    {
                        _unitOfWork.Repository<TeamMembers>().RemoveRange(membersToRemove);
                    }

                    var currentMemberIDs = existingMembers.Select(em => em.MemberID).ToList();

                    var membersToAdd = request.TeamMemberIDs.Except(currentMemberIDs).Select(memberID => new TeamMembers
                    {
                        TeamID = request.TeamID,
                        MemberID = memberID
                    }).ToList();

                    if (membersToAdd.Any())
                    {
                        await _unitOfWork.Repository<TeamMembers>().AddRange((ICollection<TeamMembers>)membersToAdd);
                    }
                }

                _unitOfWork.Repository<Team>().Update(existingTeam);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true, "Team updated successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
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
