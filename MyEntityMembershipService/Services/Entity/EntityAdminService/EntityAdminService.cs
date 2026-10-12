using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.EntityAdminDtos;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Entity.EntityAdminService
{
    public class EntityAdminService : IEntityAdminService
    {
        private const string CoachRole = "Coach";
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFetchService fetchService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public EntityAdminService(IMapper mapper, IUnitOfWork unitOfWork, IFetchService fetchService, IHttpContextAccessor httpContextAccessor)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.fetchService = fetchService;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Response<bool>> AddEntityAdmin(AddEntityAdminDto entityAdminDto)
        {
            try
            {
                var admin = mapper.Map<TenantAdmin>(entityAdminDto);
                await unitOfWork.Repository<TenantAdmin>().Add(admin);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding entity admin: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<UpdateEntityAdminDto>> UpdateEntityAdmin(UpdateEntityAdminDto entityAdminDto)
        {
            try
            {
                var spec = new GeneralSpecifications<TenantAdmin>(admin => admin.UserId == entityAdminDto.UserId)
                {
                    Includes = [
                        iq => iq.Include(admin => admin.TenantAreaAdmins),
                iq => iq.Include(e => e.TenantAdminSports),
                iq => iq.Include(e => e.TenantAdminFacilities),
                iq => iq.Include(e => e.TenantAdminVenues),
                iq => iq.Include(e => e.TenantAdminBranches),
                iq => iq.Include(e => e.TenantAdminRoles)
                    ]
                };

                var admin = await unitOfWork.Repository<TenantAdmin>().GetWithSpecifications(spec);
                if (admin is null)
                    return new ResponseHandler().BadRequest<UpdateEntityAdminDto>("There is No Admin with this Id");

                // Update basic properties
                admin.FirstName = entityAdminDto.FirstName;
                admin.LastName = entityAdminDto.LastName;
                admin.Email = entityAdminDto.Email;
                admin.PhoneNumber = entityAdminDto.PhoneNumber;
                admin.TenantId = entityAdminDto.TenantId;

                // Sports
                if (entityAdminDto.SportsIds is not null)
                {
                    admin.TenantAdminSports.Clear();
                    foreach (var sportId in entityAdminDto.SportsIds)
                        admin.TenantAdminSports.Add(new TenantAdminSport { SportId = sportId, TenantAdminId = admin.Id });
                }

                // Facilities
                if (entityAdminDto.FacilitiesIds is not null)
                {
                    admin.TenantAdminFacilities.Clear();
                    foreach (var facilityId in entityAdminDto.FacilitiesIds)
                        admin.TenantAdminFacilities.Add(new TenantAdminFacility { FacilityId = facilityId, TenantAdminId = admin.Id });
                }

                // Branches
                if (entityAdminDto.BranchsIds is not null)
                {
                        admin.TenantAdminBranches.Clear();
                    foreach (var branchId in entityAdminDto.BranchsIds)
                        admin.TenantAdminBranches.Add(new TenantAdminBranch { BranchId = branchId, TenantAdminId = admin.Id });
                }

                // Venues
                if (entityAdminDto.VenuesIds is not null)
                {
                    admin.TenantAdminVenues.Clear();
                    foreach (var venueId in entityAdminDto.VenuesIds)
                        admin.TenantAdminVenues.Add(new TenantAdminVenue { VenueId = venueId, TenantAdminId = admin.Id });
                }

                // Roles
                if (entityAdminDto.RoleIds is not null)
                {
                    admin.TenantAdminRoles.Clear();
                    foreach (var roleId in entityAdminDto.RoleIds)
                        admin.TenantAdminRoles.Add(new TenantAdminRole { RoleId = roleId, TenantAdminId = admin.Id });
                }

                unitOfWork.Repository<TenantAdmin>().Update(admin);
                await unitOfWork.Save();

                return new ResponseHandler().Success(entityAdminDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating entity admin: {Message}. Inner: {InnerMessage}",
                    ex.Message, ex.InnerException?.Message);
                return new ResponseHandler().BadRequest<UpdateEntityAdminDto>($"Something went wrong: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public async Task<Response<List<GetEntityAdminDto>>> GetEntityAdmins(GetEntityAdminsParametersDto parameters)
        {
            try
            {
                var totalAdmins = await unitOfWork.Repository<TenantAdmin>().FindAllByCriteriaAsync(admin => admin.TenantId == parameters.TenantId &&
                (!parameters.Status.HasValue || admin.EntityAdminStatus == (EntityAdminStatus)parameters.Status) &&
                (string.IsNullOrEmpty(parameters.SearchBy) || admin.FirstName.ToLower().Contains(parameters.SearchBy.ToLower()) ||
                admin.LastName.ToLower().Contains(parameters.SearchBy.ToLower()) || admin.PhoneNumber.Contains(parameters.SearchBy)));

                var spec = new GeneralSpecifications<TenantAdmin>(admin => admin.TenantId == parameters.TenantId && 
                (!parameters.Status.HasValue || admin.EntityAdminStatus == (EntityAdminStatus) parameters.Status) &&
                (string.IsNullOrEmpty(parameters.SearchBy) || admin.FirstName.ToLower().Contains(parameters.SearchBy.ToLower()) || 
                admin.LastName.ToLower().Contains(parameters.SearchBy.ToLower()) || admin.PhoneNumber.Contains(parameters.SearchBy) ),
                parameters.PageIndex, parameters.PageSize)
                {
                    Includes = [
                        iq => iq.Include(admin => admin.TenantAdminRoles)
                        .Include(admin => admin.TenantAdminSports)
                        .Include(admin => admin.TenantAdminFacilities)
                        .Include(admin => admin.TenantAdminBranches)
                        .Include(admin => admin.TenantAdminVenues)
                        .Include(admin => admin.TenantAreaAdmins)
                        .ThenInclude(area => area.Area),
                        ]
                };
                var admins = await unitOfWork.Repository<TenantAdmin>().GetAllWithSpecifications(spec);
                var adminsDto = mapper.Map<List<GetEntityAdminDto>>(admins);
                if (adminsDto.Count == 0)
                    return new ResponseHandler().Success(new List<GetEntityAdminDto>());
               
                return new ResponseHandler().Success(adminsDto , new { NumberOfPages = (int)Math.Ceiling((double)totalAdmins.Count / parameters.PageSize) });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving entity admins: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<List<GetEntityAdminDto>>("Something went wrong");
            }
        }

        public async Task<Response<List<TenantCoachAdminDto>>> GetTenantCoachAdmins(
            int tenantId,
            GetTenantCoachAdminsParametersDto parameters)
        {
            if (tenantId <= 0)
                return new ResponseHandler().BadRequest<List<TenantCoachAdminDto>>("Invalid tenant ID");

            if (parameters.PageIndex <= 0 || parameters.PageSize <= 0)
                return new ResponseHandler().BadRequest<List<TenantCoachAdminDto>>("PageIndex and PageSize must be greater than zero");

            try
            {
                var normalizedSearch = parameters.Search?.Trim().ToLower();

                Expression<Func<TenantAdmin, bool>> criteria = admin =>
                    admin.TenantId == tenantId &&
                    admin.EntityAdminStatus == EntityAdminStatus.Active &&
                    admin.TenantAdminRoles.Any(role => role.RoleId == CoachRole) &&
                    (string.IsNullOrEmpty(normalizedSearch) ||
                        (admin.FirstName ?? string.Empty).ToLower().Contains(normalizedSearch) ||
                        (admin.LastName ?? string.Empty).ToLower().Contains(normalizedSearch) ||
                        ((admin.FirstName ?? string.Empty) + " " + (admin.LastName ?? string.Empty)).ToLower().Contains(normalizedSearch) ||
                        (admin.Email ?? string.Empty).ToLower().Contains(normalizedSearch));

                var totalCount = await unitOfWork.Repository<TenantAdmin>().GetCountByCriteriaAsync(criteria);
                var specification = new GeneralSpecifications<TenantAdmin>(
                    criteria,
                    parameters.PageIndex,
                    parameters.PageSize);

                specification.AsNoTracking = true;
                specification.SetOrderBy(admin => admin.FirstName);

                var coaches = await unitOfWork.Repository<TenantAdmin>()
                    .GetAllWithSpecificationsAndProjection(
                        specification,
                        admin => new TenantCoachAdminDto
                        {
                            Id = admin.UserId,
                            userName = ((admin.FirstName ?? string.Empty) + " " + (admin.LastName ?? string.Empty)).Trim(),
                            Number = admin.PhoneNumber,
                            Email = admin.Email
                        });

                var numberOfPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

                return new ResponseHandler().Success(coaches, new
                {
                    parameters.PageIndex,
                    parameters.PageSize,
                    TotalCount = totalCount,
                    NumberOfPages = numberOfPages
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving coach admins for tenant {TenantId}", tenantId);
                return new ResponseHandler().BadRequest<List<TenantCoachAdminDto>>("Something went wrong");
            }
        }

        public async Task<Response<List<GetEntityAdminDto>>> GetFederationEntitiesAdmins(GetFederationEntitiesAdminsParametersDto parameters)
        {
            try
            {

                var tenantAdmins = await fetchService.GetTenantAdmin(httpContextAccessor.HttpContext.User.GetUserId());
                 GeneralSpecifications<Tenant> FederationSpec;

               
                var areasId = tenantAdmins.TenantAreaAdmins.Select(area => area.AreaId).ToList();

                FederationSpec = new GeneralSpecifications<Tenant>(federation => federation.Id == parameters.FederationId && ( !areasId.Any() || federation.Entites.Any(t => t.TenantAdmins.Any(admin => admin.TenantAreaAdmins.Any(area => areasId.Contains(area.AreaId))))))
                {
                    Includes = [
                        iq => iq.Include(e => e.Entites.Where(ent =>
                            ent.TenantAdmins.Any(a =>
                            (!areasId.Any() || a.TenantAreaAdmins.Any(ta => areasId.Contains(ta.AreaId))) &&
                            (!parameters.BranchId.HasValue || ent.AreaId == parameters.BranchId) && // Filter By Branch Added
                            (!parameters.Status.HasValue || a.EntityAdminStatus == (EntityAdminStatus)parameters.Status) && // Filter By Status Added 
                            (string.IsNullOrEmpty(parameters.SearchBy) || a.FirstName.ToLower().Contains(parameters.SearchBy.ToLower()) || a.LastName.ToLower().Contains(parameters.SearchBy.ToLower()) || a.PhoneNumber.Contains(parameters.SearchBy) || a.Email.Contains(parameters.SearchBy)) // SearchBy Name , Mobile Number , Email
                            ) ))
                            .ThenInclude(t => t.TenantAdmins)
                            .ThenInclude(t => t.TenantAreaAdmins)
                            .ThenInclude(t => t.Area)
                            .Include(e =>e.Entites).ThenInclude(t => t.TenantAdmins).ThenInclude(t => t.TenantAdminRoles)
                            
                            ]
                };
                var federation = await unitOfWork.Repository<Tenant>().GetWithSpecifications(FederationSpec);
                var admins = mapper.Map<List<GetEntityAdminDto>>(federation.Entites.SelectMany(t => t.TenantAdmins));


                var totalCount = admins.Count;
                var result = admins.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
                
                return new ResponseHandler().Success(result, new {NumberOfPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize)  });
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error retrieving federation entity admins: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<List<GetEntityAdminDto>>("Something went wrong");
            }
        }

        public async Task<Response<GetEntityAdminDto>> UpdateEntityAdminStatus(string UserId)
        {
            try
            {
                var spec = new GeneralSpecifications<TenantAdmin>(admin => admin.UserId == UserId)
                {
                    Includes = [
                        iq => iq.Include(admin => admin.TenantAreaAdmins).ThenInclude(area => area.Area),
                        ]
                };
                var entityAdmin = await unitOfWork.Repository<TenantAdmin>().GetWithSpecifications(spec);
                if (entityAdmin is null)
                    return new ResponseHandler().Success(new GetEntityAdminDto() { UserId = UserId, EntityType = TenantType.BuildUpUser.ToString() });
                entityAdmin.EntityAdminStatus = EntityAdminStatus.Active;
                unitOfWork.Repository<TenantAdmin>().Update(entityAdmin);
                await unitOfWork.Save();
                var result = mapper.Map<GetEntityAdminDto>(entityAdmin);
                var entity = await unitOfWork.Repository<Tenant>().GetByIdAsync(result.TenantId);
                result.TenantName = entity?.Name ?? "UNKNOWN";
                result.Dashboard = entity?.Dashboard ?? (Dashboard)1;
                if (entity.SuperEntityId is not null)
                    result.EntityType = TenantType.Entity.ToString();
                else
                    result.EntityType = TenantType.Federation.ToString();
                
                return new ResponseHandler().Success(result);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error updating entity admin status: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<GetEntityAdminDto>("Something went wrong");
            }
        }

        public async Task<Response<string>> DeactiveEntityAdmin(string userId)
        {
            try
            {
                var spec = new GeneralSpecifications<TenantAdmin>(t => t.UserId == userId);
                
                var entityAdmin = await unitOfWork.Repository<TenantAdmin>().GetWithSpecifications(spec);
                if (entityAdmin is null) {
                    Log.Warning("Entity admin with UserId {UserId} not found.", userId);
                    return new ResponseHandler().NotFound<string>("Entity admin not found.");
                }


                entityAdmin.EntityAdminStatus = EntityAdminStatus.Deactivated;

                unitOfWork.Repository<TenantAdmin>().Update(entityAdmin);
                await unitOfWork.Save();

                Log.Information("Entity admin with UserId {UserId} deactivated successfully.", userId);
                return new ResponseHandler().Success("Entity admin deactivated successfully.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deactivating entity admin: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<string>("Failed to deactivate entity admin.");
            }
        }

        public async Task<Response<TenantAdminDto>> GetTenantAdminByUserId(string userId)
        {
            try
            {
                var tenantSpec = new GeneralSpecifications<TenantAdmin>(
                    t => t.UserId == userId
                    );
                
                tenantSpec.AddInclude(q => q.Include( t=> t.TenantAdminBranches));
                tenantSpec.AddInclude(q => q.Include(t => t.TenantAdminFacilities));
                tenantSpec.AddInclude(q => q.Include(t => t.TenantAdminSports));
                tenantSpec.AddInclude(q => q.Include(t => t.TenantAdminVenues));

                var tenantAdmin = await unitOfWork.Repository<TenantAdmin>().GetWithSpecifications(tenantSpec);
                
                if (tenantAdmin is null) {
                    Log.Warning("Tenant admin with UserId {UserId} not found.", userId);
                    return new ResponseHandler().NotFound<TenantAdminDto>($"Tenant admin with userId {userId} not found.");
                }

                var tenantAdminDto = mapper.Map<TenantAdminDto>(tenantAdmin);
                return new ResponseHandler().Success(tenantAdminDto);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting TenantAdmin by UserId: {UserId}", userId);
                return new ResponseHandler().BadRequest<TenantAdminDto>($"Failed to get tenant admin for userId {userId}.");

            }
        }
    }
}
