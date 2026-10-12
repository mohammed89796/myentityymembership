using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.DTOs.Membership.FederationEntitiesDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.SpecificationPattern;


namespace MyEntityMembershipService.Services.Membership.EntityService
{
    public class EntityService : IEntityService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IFetchService fetchService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public EntityService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IFetchService fetchService, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configuration = configuration;
            this.fetchService = fetchService;
            this.httpContextAccessor = httpContextAccessor;
        }


        public async Task<Response<EntityDto>> CreateFederationEntity(EntityDto dto)
        {
            try
            {
                var entity = mapper.Map<Tenant>(dto);
                entity.EntityTypeId = 1;
                entity.CreationDate = DateTime.UtcNow;
                await unitOfWork.Repository<Tenant>().Add(entity);
                await unitOfWork.Save();
                var result = mapper.Map<EntityDto>(entity);
                return new ResponseHandler().Success(result);
            }
            catch
            {
                return new ResponseHandler().BadRequest<EntityDto>("Failed to create federation entity.");
            }
        }

        public async Task<Response<PagedResult<EntityDto>>> GetFederationEntitiesAsync(GetFederationEntitiesParameterDto parameters)
        {
            try
            {
                // FilterationAccording For Admin
                var entityAdmin = await fetchService.GetTenantAdmin(parameters.userId); // Should be replaced with FetchService

                // if entity for user is not Federation 
                if (entityAdmin.Tenant.SuperEntityId is not null)
                {

                    var entityAdminArea = entityAdmin.TenantAreaAdmins.FirstOrDefault();
                    if (entityAdminArea is not null)
                    {
                        var spec = new GeneralSpecifications<Tenant>(entity => entity.SuperEntityId == parameters.tenentId && entity.AreaId == entityAdminArea.AreaId &&
                        (!parameters.BranchId.HasValue || entity.AreaId == parameters.BranchId) &&
                        (!parameters.EntityTypeId.HasValue || entity.FedrationEntityTypeId == parameters.EntityTypeId) &&
                        (string.IsNullOrEmpty(parameters.SearchBy) || entity.Name.ToLower().Contains(parameters.SearchBy.ToLower())),
                        parameters.pageNumber.Value, parameters.pageSize.Value);
                        spec.Includes = new List<Func<IQueryable<Tenant>, IQueryable<Tenant>>>
                        {
                            entity => entity.Include(e => e.FedrationEntityType)
                        };
                        var entities = await unitOfWork.Repository<Tenant>().GetAllWithSpecifications(spec);
                        var allentities = await unitOfWork.Repository<Tenant>().GetCountByCriteriaAsync(e => e.SuperEntityId == parameters.tenentId && e.AreaId == entityAdminArea.AreaId);
                        var entitiesDto = mapper.Map<List<EntityDto>>(entities);

                        var tasks = entitiesDto.Select(async dto =>
                        {
                            dto.Image = await DocumentSetting.GetPreSignedUrlAsync(

                                dto.Image

                                );
                        }).ToList();
                        await Task.WhenAll(tasks);

                        var result = new PagedResult<EntityDto>
                        {
                            TotalPages = allentities > 0 ? (int)Math.Ceiling(allentities / (double)parameters.pageSize.Value) : 0,
                            PageNumber = parameters.pageNumber.Value,
                            PageSize = parameters.pageSize.Value,
                            Items = entitiesDto
                        };

                        return new ResponseHandler().Success(result);
                    }
                }

                int safePageSize = parameters.pageSize.GetValueOrDefault(10);
                int safePageNumber = parameters.pageNumber.GetValueOrDefault(1);

                if (safePageSize <= 0) safePageSize = 10;
                if (safePageNumber <= 0) safePageNumber = 1;
                var areasId = entityAdmin.TenantAreaAdmins.Select(areaAdmin => areaAdmin.AreaId).ToList();

                GeneralSpecifications<Tenant> entitySpecifications;


                entitySpecifications = new GeneralSpecifications<Tenant>(
                    f => f.SuperEntityId == parameters.tenentId
                    && (!areasId.Any() || (!f.AreaId.HasValue || areasId.Contains(f.AreaId.Value))) &&
                    (!parameters.BranchId.HasValue || f.AreaId == parameters.BranchId) &&
                        (!parameters.EntityTypeId.HasValue || f.FedrationEntityTypeId == parameters.EntityTypeId) &&
                        (string.IsNullOrEmpty(parameters.SearchBy) || f.Name.ToLower().Contains(parameters.SearchBy.ToLower()))
                    , safePageNumber,
                    safePageSize)
                {
                    Includes = new List<Func<IQueryable<Tenant>, IQueryable<Tenant>>>
                        {
                            q => q.Include(r => r.FedrationEntityType),
                            q => q.Include(r => r.Area)



                        }
                };



                // Get paged and filtered data
                var entity = await unitOfWork.Repository<Tenant>().GetAllWithSpecifications(entitySpecifications);


                var totalCount = await unitOfWork.Repository<Tenant>().GetCountByCriteriaAsync(f => f.SuperEntityId == parameters.tenentId &&
                        (!parameters.BranchId.HasValue || f.AreaId == parameters.BranchId) &&
                        (!parameters.EntityTypeId.HasValue || f.FedrationEntityTypeId == parameters.EntityTypeId) &&
                        (string.IsNullOrEmpty(parameters.SearchBy) || f.Name.ToLower().Contains(parameters.SearchBy.ToLower())));


                var federationDtos = mapper.Map<List<EntityDto>>(entity);

                var pagedResult = new PagedResult<EntityDto>
                {
                    TotalPages = totalCount > 0 ? (int)Math.Ceiling(totalCount / (double)safePageSize) : 0,
                    PageNumber = safePageNumber,
                    PageSize = safePageSize,
                    Items = federationDtos
                };

                return new ResponseHandler().Success(pagedResult);
            }
            catch (Exception)
            {
                return new ResponseHandler().BadRequest<PagedResult<EntityDto>>("Failed to retrieve federations.");
            }
        }

        public async Task<Response<EntityDto>> EditFederationEntity(UpdateEntityDto dto)
        {
            try
            {
                var entity = await unitOfWork.Repository<Tenant>().GetByIdAsync(dto.Id);
                if (dto.Image != null)
                {
                    entity.Image = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", dto.Image);
                }
                if (entity == null || entity.IsDeleted)
                    return new ResponseHandler().NotFound<EntityDto>("Entity not found.");

                mapper.Map(dto, entity);
                unitOfWork.Repository<Tenant>().Update(entity);
                await unitOfWork.Save();
                var result = mapper.Map<EntityDto>(entity);
                return new ResponseHandler().Success(result);
            }
            catch
            {
                return new ResponseHandler().BadRequest<EntityDto>("Failed to update entity.");
            }
        }

        public async Task<Response<string>> DeleteFederationEntity(int id)
        {
            try
            {
                var entity = await unitOfWork.Repository<Tenant>().GetByIdAsync(id);
                if (entity == null || entity.IsDeleted)
                    return new ResponseHandler().NotFound<string>("Entity not found.");

                entity.IsDeleted = true;
                unitOfWork.Repository<Tenant>().SoftDelete(entity);
                await unitOfWork.Save();
                return new ResponseHandler().Success("Entity deleted successfully.");
            }
            catch
            {
                return new ResponseHandler().BadRequest<string>("Failed to delete entity.");
            }
        }

        public async Task<Response<EntityDto>> GetFederationEntityByIdAsync(int id)
        {
            try
            {
                var entity = await unitOfWork.Repository<Tenant>().GetWithIncludesAsync(
                    id, "Id", f => f.FedrationEntityType, f => f.Area

                );

                if (entity == null || entity.IsDeleted)
                    return new ResponseHandler().NotFound<EntityDto>("Federation entity not found.");

                var dto = mapper.Map<EntityDto>(entity);

                dto.Image = await DocumentSetting.GetPreSignedUrlAsync(
                    dto.Image
                    );

                return new ResponseHandler().Success(dto);
            }
            catch
            {
                return new ResponseHandler().BadRequest<EntityDto>("Failed to retrieve federation entity.");
            }
        }

        public async Task<Response<EntityCountsDto>> GetEntityCountsAsync(int tenantId)
        {
            try
            {

                var userId = httpContextAccessor.HttpContext.User.GetUserId();
                var admin = await fetchService.GetTenantAdmin(userId);
                var areaIds = admin.TenantAreaAdmins.Select(area => area.AreaId).ToList();
                GeneralSpecifications<Tenant> specification;
                specification = new GeneralSpecifications<Tenant>(
                    f => f.SuperEntityId == tenantId
                    && (!areaIds.Any() || areaIds.Contains(f.AreaId ?? 0))
                )
                {
                    Includes = new List<Func<IQueryable<Tenant>, IQueryable<Tenant>>>
                    {
                        q => q.Include(r => r.FedrationEntityType),
                        q => q.Include(r => r.Area)
                    }
                };


                var entities = await unitOfWork.Repository<Tenant>().GetAllWithSpecifications(specification);

                var entityCountsDto = new EntityCountsDto
                {
                    NumberOfEntities = entities.Count,
                    NumberOfAcademies = entities.Count(e => e.FedrationEntityTypeId == 1), // Academy
                    NumberOfClubs = entities.Count(e => e.FedrationEntityTypeId == 2),     // Club
                    YouthClubs = entities.Count(e => e.FedrationEntityTypeId == 6),        // Youth Center
                    TrainingCenters = entities.Count(e => e.FedrationEntityTypeId == 5)    // Training Center
                };

                return new ResponseHandler().Success(entityCountsDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EntityCountsDto>("Failed to retrieve entity statistics.");

            }
        }
    }
}

