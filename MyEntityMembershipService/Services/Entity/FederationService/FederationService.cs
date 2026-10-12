using AutoMapper;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.FederationDtos;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Entity.FederationService
{
    public class FederationService : IFederationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public FederationService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configuration = configuration;
        }


        public async Task<Response<FederationEntityDto>> CreateFederationEntity(AddFederationEntityDto dto)
        {
            try
            {
                var entity = mapper.Map<Tenant>(dto);



                entity.Image = dto.Image != null ? await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", dto.Image) : null;
                entity.EntityTypeId = 1;
                entity.CreationDate = DateTime.UtcNow;
                await unitOfWork.Repository<Tenant>().Add(entity);
                await unitOfWork.Save();
                var result = mapper.Map<FederationEntityDto>(entity);
                return new ResponseHandler().Success(result);
            }
            catch
            {
                return new ResponseHandler().BadRequest<FederationEntityDto>("Failed to create federation entity.");
            }
        }
        public async Task<Response<FederationDto>> CreateFederation(AddFederationDto dto)
        {
            try
            {
                var entity = mapper.Map<Tenant>(dto);



                entity.Image = dto.Image != null ? await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", dto.Image) : null;
                entity.EntityTypeId = 2;
                entity.CreationDate = DateTime.UtcNow;
                await unitOfWork.Repository<Tenant>().Add(entity);
                await unitOfWork.Save();
                var result = mapper.Map<FederationDto>(entity);
                return new ResponseHandler().Success(result);
            }
            catch
            {
                return new ResponseHandler().BadRequest<FederationDto>("Failed to create federation.");
            }
        }

        public async Task<Response<PagedResult<FederationEntityDto>>> GetFederationEntitiesAsync(int tenentId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                int safePageSize = pageSize.GetValueOrDefault(10);
                int safePageNumber = pageNumber.GetValueOrDefault(1);

                if (safePageSize <= 0) safePageSize = 10;
                if (safePageNumber <= 0) safePageNumber = 1;



                // Get paged and filtered data
                var federations = await unitOfWork.Repository<Tenant>().GetAllWithIncludesAsync(
                         f => f.SuperEntityId == tenentId,
                         safePageSize,
                         safePageNumber,
                         f => f.EntityType,
                         f => f.FedrationEntityType
                     );

                var totalCount = await unitOfWork.Repository<Tenant>().GetCountByCriteriaAsync(f => f.SuperEntityId == tenentId);


                var federationDtos = mapper.Map<List<FederationEntityDto>>(federations);

                var tasks = federationDtos.Select(async dto =>
                {
                    dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);
                });

                await Task.WhenAll(tasks);

                var pagedResult = new PagedResult<FederationEntityDto>
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
                return new ResponseHandler().BadRequest<PagedResult<FederationEntityDto>>("Failed to retrieve federations.");
            }
        }


        public async Task<Response<FederationEntityDto>> EditFederationEntity(UpdateFederationDto dto)
        {
            try
            {
                var entity = await unitOfWork.Repository<Tenant>().GetByIdAsync(dto.Id);

                if (entity == null || entity.IsDeleted)
                    return new ResponseHandler().NotFound<FederationEntityDto>("Entity not found.");

                if (dto.Image != null)
                {
                    entity.Image = await DocumentSetting.AddToContainerAndGetUrl(
                        Environment.GetEnvironmentVariable("StorageConnectionString"),
                        "buildupimagedocs",
                        dto.Image
                    );
                }

                mapper.Map(dto, entity);
                unitOfWork.Repository<Tenant>().Update(entity);
                await unitOfWork.Save();

                // Perform ExecuteUpdate on related tables:
                string updatedName = entity.Name;

                // 1. Update Batch
                await unitOfWork.Repository<Batch>()
                    .ExecuteUpdateAsync(
                        b => b.TenantId == dto.Id,
                        setters => setters.SetProperty(b => b.TenantName, _ => updatedName)
                    );

                await unitOfWork.Repository<PaymentBatch>()
                    .ExecuteUpdateAsync(
                        p => p.EntityId == dto.Id,
                        setters => setters.SetProperty(p => p.EntityName, _ => updatedName)
                    );

                await unitOfWork.Repository<NewPaymentRequest>()
                    .ExecuteUpdateAsync(
                        n => n.EntityId == dto.Id,
                        setters => setters.SetProperty(n => n.EntityName, _ => updatedName)
                    );


                var result = mapper.Map<FederationEntityDto>(entity);
                return new ResponseHandler().Success(result);
            }
            catch
            {
                return new ResponseHandler().BadRequest<FederationEntityDto>("Failed to update entity.");
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

        public async Task<Response<FederationEntityDto>> GetFederationEntityByIdAsync(int id)
        {
            try
            {
                var entity = await unitOfWork.Repository<Tenant>().GetWithIncludesAsync(
                    id,
                    f => f.EntityType,
                    f => f.FedrationEntityType
                );

                if (entity == null || entity.IsDeleted)
                    return new ResponseHandler().NotFound<FederationEntityDto>("Federation entity not found.");

                var dto = mapper.Map<FederationEntityDto>(entity);

                dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);
                return new ResponseHandler().Success(dto);
            }
            catch
            {
                return new ResponseHandler().BadRequest<FederationEntityDto>("Failed to retrieve federation entity.");
            }
        }

    }
}
