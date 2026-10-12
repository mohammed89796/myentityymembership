using AutoMapper;
using Common.RabbitMq.Email;
using Common.RabbitMq.Tenant;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;

using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.TenantRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Entity.TenantService
{
    public class TenantService : ITenantService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private ITenantRepository tenantRepository;
        private IConfiguration configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IFetchService fetchService;

        public TenantService(IUnitOfWork unitOfWork, IMapper mapper, ITenantRepository tenantRepository, IConfiguration configuration, IPublishEndpoint publishEndpoint, IFetchService fetchService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.tenantRepository = tenantRepository;
            this.configuration = configuration;
            _publishEndpoint = publishEndpoint;
            this.fetchService = fetchService;
        }
        public async Task<Repositories.Common.Response<AddTenantDto>> AddTenant(AddTenantDto addTenantDto)
        {
            try
            {
                var tenant = mapper.Map<Tenant>(addTenantDto);
                tenant.CreationDate = DateTime.Now;
                await unitOfWork.Repository<Tenant>().Add(tenant);
                await unitOfWork.Save();

                // publish event to create default packages for the new tenant
                await _publishEndpoint.Publish
                    (new TenantCreatedDto
                    {
                        EntityId = tenant.Id,
                        Name = tenant.Name,
                        ImageUrl = tenant.Image!
                    });


                addTenantDto = mapper.Map<AddTenantDto>(tenant);
                return new ResponseHandler().Success(addTenantDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AddTenantDto>("Something went wrong");
            }
        }

        public async Task<Repositories.Common.Response<List<AddTenantDto>>> GetAllTenants(string? searchName)
        {
            try
            {
                var requests = await tenantRepository.GetAllTenantsAsync(1);
                requests = string.IsNullOrEmpty(searchName) ? requests : requests.Where(r => r.Name.ToLower().Contains(searchName.ToLower())).ToList();
                var result = mapper.Map<List<AddTenantDto>>(requests);

                var tasks = result.Select(async tenantDto =>
                {
                    if (!string.IsNullOrEmpty(tenantDto.Image))
                    {
                        tenantDto.Image = await DocumentSetting.GetPreSignedUrlAsync(tenantDto.Image);
                    }
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<AddTenantDto>>("Something went wrong");
            }
        }
        public async Task<Repositories.Common.Response<List<AddTenantDto>>> GetAllFederationss(string? searchName)
        {
            try
            {
                var requests = await tenantRepository.GetAllFedrationsAsync(1);
                requests = string.IsNullOrEmpty(searchName) ? requests : requests.Where(r => r.Name.ToLower().Contains(searchName.ToLower())).ToList();
                var result = mapper.Map<List<AddTenantDto>>(requests);

                var tasks = result.Select(async tenantDto =>
                {
                    if (!string.IsNullOrEmpty(tenantDto.Image))
                    {
                        tenantDto.Image = await DocumentSetting.GetPreSignedUrlAsync(tenantDto.Image);
                    }
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<AddTenantDto>>("Something went wrong");
            }
        }

        public async Task<Repositories.Common.Response<TenantDto>> GetTenantByIdAsync(int tenantId)
        {
            try
            {
                var tenant = await tenantRepository.GetTenantByIdAsync(tenantId, 1);


                if (tenant is not null)
                {

                    var mappedTenant = mapper.Map<Tenant, TenantDto>(tenant);

                    mappedTenant.Image = await DocumentSetting.GetPreSignedUrlAsync(mappedTenant.Image);

                    var branchTasks =
                        mappedTenant.Branches
                        .Where(b => !string.IsNullOrEmpty(b.BranchImageURL))
                        .Select(async b =>
                        {
                            b.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(b.BranchImageURL);
                        }).ToList();

                    var facilityTasks =
                        mappedTenant.Branches
                        .SelectMany(b => b.Facilities)
                        .Where(f => !string.IsNullOrEmpty(f.Image))
                        .Select(async f =>
                        {
                            f.Image = await DocumentSetting.GetPreSignedUrlAsync(f.Image);
                        }).ToList();

                    var venueTasks =
                        mappedTenant.Branches
                        .SelectMany(b => b.Facilities)
                        .SelectMany(f => f.Venues)
                        .Where(v => !string.IsNullOrEmpty(v.VenueImageURL))
                        .Select(async v =>
                        {
                            v.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(v.VenueImageURL);
                        }).ToList();


                    await Task.WhenAll(branchTasks.Concat(facilityTasks).Concat(venueTasks));

                    var result = await fetchService.EntityBlockedAsync(tenantId);
                    if (result.Succeeded)
                    {
                        mappedTenant.IsBlocked = result.Data;
                    }
                    return new ResponseHandler().Success(mappedTenant);
                }
                else
                {
                    return new ResponseHandler().NotFound<TenantDto>("Invalid tenantId");
                }


            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<TenantDto>("Something went wrong");
            }
        }

        public async Task<Repositories.Common.Response<TenantDto>> GetFedByIdAsync(int tenantId)
        {
            try
            {
                var tenant = await tenantRepository.GetTenantByIdAsync(2, 1);

                if (tenant is not null)
                {

                    var mappedTenant = mapper.Map<Tenant, TenantDto>(tenant);

                    mappedTenant.Image = await DocumentSetting.GetPreSignedUrlAsync(mappedTenant.Image);

                    var branchTasks =
                        mappedTenant.Branches
                        .Where(b => !string.IsNullOrEmpty(b.BranchImageURL))
                        .Select(async b =>
                        {
                            b.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(b.BranchImageURL);
                        }).ToList();

                    var facilityTasks =
                        mappedTenant.Branches
                        .SelectMany(b => b.Facilities)
                        .Where(f => !string.IsNullOrEmpty(f.Image))
                        .Select(async f =>
                        {
                            f.Image = await DocumentSetting.GetPreSignedUrlAsync(f.Image);
                        }).ToList();

                    var venueTasks =
                        mappedTenant.Branches
                        .SelectMany(b => b.Facilities)
                        .SelectMany(f => f.Venues)
                        .Where(v => !string.IsNullOrEmpty(v.VenueImageURL))
                        .Select(async v =>
                        {
                            v.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(v.VenueImageURL);
                        }).ToList();


                    await Task.WhenAll(branchTasks.Concat(facilityTasks).Concat(venueTasks));

                    return new ResponseHandler().Success(mappedTenant);
                }
                else
                {
                    return new ResponseHandler().NotFound<TenantDto>("Invalid Fedration");
                }


            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<TenantDto>("Something went wrong");
            }
        }


        public async Task<Repositories.Common.Response<UpdateTenantDto>> UpdateTenant(UpdateTenantDto updateTenantDto)
        {
            try
            {
                var tenant = await tenantRepository.GetTenantByIdAsync(updateTenantDto.Id, 1);
                if (tenant == null)
                {
                    return new ResponseHandler().NotFound<UpdateTenantDto>("Tenant not found");
                }

                if (updateTenantDto.ImageFile != null)
                {
                    updateTenantDto.ImageUrl = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", updateTenantDto.ImageFile);
                }


                mapper.Map(updateTenantDto, tenant);
                if (updateTenantDto.ImageUrl != null)
                {
                    tenant.Image = updateTenantDto.ImageUrl;

                }
                if (updateTenantDto.SocialMediaLinks != null && updateTenantDto.SocialMediaLinks.Any())
                {
                    tenant.SocialMediaLinks = updateTenantDto.SocialMediaLinks;
                }

                unitOfWork.Repository<Tenant>().Update(tenant);
                await unitOfWork.Save();

                var updatedTenantDto = mapper.Map<UpdateTenantDto>(tenant);

                updateTenantDto.ImageUrl = tenant.Image;

                // publish event to update default packages for the updated tenant
                await _publishEndpoint.Publish(new TenantUpdatedDto
                {
                    EntityId = tenant.Id,
                    Name = tenant.Name,
                    ImageUrl = tenant.Image!
                });

                return new ResponseHandler().Success(updatedTenantDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<UpdateTenantDto>("Something went wrong");
            }
        }

        public async Task<Repositories.Common.Response<IReadOnlyList<AddTenantDto>>> GetTenantsByListOfIdsAsync(List<int> tenantIds)
        {
            try
            {
                var tenants = await tenantRepository.GetTenantsByListOfIdsAsync(tenantIds);

                if (tenants.Count > 0)
                {

                    var mappedTenants = mapper.Map<IReadOnlyList<Tenant>, IReadOnlyList<AddTenantDto>>(tenants);

                    var tasks = mappedTenants.Select(async tenantDto =>
                    {
                        if (!string.IsNullOrEmpty(tenantDto.Image))
                        {
                            tenantDto.Image = await DocumentSetting.GetPreSignedUrlAsync(tenantDto.Image);
                        }
                    });

                    await Task.WhenAll(tasks);
                    return new ResponseHandler().Success(mappedTenants);
                }
                else
                    return new ResponseHandler().NotFound<IReadOnlyList<AddTenantDto>>();
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<IReadOnlyList<AddTenantDto>>("Something went wrong");
            }

        }
        public async Task<Repositories.Common.Response<string>> CheckTanantTypeById(int tenantId)
        {
            try
            {
                var tenantType = await tenantRepository.CheckEntityType(tenantId);

                if (tenantType is null)
                {

                    return new ResponseHandler().BadRequest<string>("Unable to fetch entity type");

                }
                return new ResponseHandler().Success(tenantType);
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<string>("Something went wrong");
            }

        }

        public async Task<Repositories.Common.Response<TenantDetailsDto>> GetTenantDetails(int tenantId)
        {
            try
            {
                var tenant = await unitOfWork.Repository<Tenant>()
                         .GetWithIncludesAsync(
                              tenantId,
                             t => t.Packages,
                            t => t.SuperEntity,
                           s => s.SuperEntity.Packages
                         );



                if (tenant == null)
                    return new ResponseHandler().NotFound<TenantDetailsDto>("Tenant not found.");

                var tenantDto = mapper.Map<TenantDetailsDto>(tenant);

                if (tenant.SuperEntity != null)
                    tenantDto.SuperEntity = mapper.Map<TenantDetailsDto>(tenant.SuperEntity);

                tenantDto.Image = await DocumentSetting.GetPreSignedUrlAsync(tenantDto.Image);

                var packageImageTasks = tenantDto.Packages
                    .Where(p => !string.IsNullOrEmpty(p.Image))
                    .Select(async p =>
                    {
                        p.Image = await DocumentSetting.GetPreSignedUrlAsync(p.Image);
                    }).ToList();

                if (tenantDto.SuperEntity != null)
                    tenantDto.SuperEntity.Image = await DocumentSetting.GetPreSignedUrlAsync(tenantDto.SuperEntity.Image);

                var superEntityPackageImageTasks = tenantDto.SuperEntity?.Packages
                    .Where(p => !string.IsNullOrEmpty(p.Image))
                    .Select(async p =>
                    {
                        p.Image = await DocumentSetting.GetPreSignedUrlAsync(p.Image);
                    }).ToList() ?? new List<Task>();

                await Task.WhenAll(packageImageTasks.Concat(superEntityPackageImageTasks).ToList());

                var result = await fetchService.EntityBlockedAsync(tenantId);
                if (result.Succeeded)
                {
                    Log.Information("EntityBlockedAsync succeeded for tenantId {TenantId} with result: {IsBlocked}", tenantId, result.Data);
                    tenantDto.IsBlocked = result.Data;
                }

                return new ResponseHandler().Success(tenantDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<TenantDetailsDto>(ex.Message);
            }
        }

        public async Task<Repositories.Common.Response<List<AddTenantDto>>> GetFederationEntities(int Id)
        {
            try
            {
                var entities = await unitOfWork.Repository<Tenant>().FindAllByCriteriaAsync(tenant => tenant.SuperEntityId == Id);
                var entitiesDto = mapper.Map<List<AddTenantDto>>(entities);

                var tasks = entitiesDto.Select(async tenantDto =>
                {
                    if (!string.IsNullOrEmpty(tenantDto.Image))
                    {
                        tenantDto.Image = await DocumentSetting.GetPreSignedUrlAsync(tenantDto.Image);
                    }
                }).ToList();
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(entitiesDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<AddTenantDto>>("Something went wrong");
            }
        }

        public async Task<Repositories.Common.Response<List<GetBranchEntityDto>>> GetEntitiesByArea(GetEntitiesByAreaParametersDto parameters)
        {
            try
            {
                var totalCount = await unitOfWork.Repository<Tenant>().GetCountByCriteriaAsync(entity => entity.FedrationBranchId == parameters.AreaId && entity.SuperEntityId == parameters.FederationId);
                // if they doesn't send pagination retieve all in first page  for drop down 
                if (!parameters.PageIndex.HasValue && !parameters.PageSize.HasValue)
                {
                    parameters.PageIndex = 1;
                    parameters.PageSize = totalCount;
                }
                var spec = new GeneralSpecifications<Tenant>(entity => entity.FedrationBranchId == parameters.AreaId && entity.SuperEntityId == parameters.FederationId, parameters.PageIndex.Value, parameters.PageSize.Value);
                spec.AddInclude(entity => entity.Include(entity => entity.FedrationEntityType));
                var entities = await unitOfWork.Repository<Tenant>().GetAllWithSpecifications(spec);
                var entitiesDto = mapper.Map<List<GetBranchEntityDto>>(entities);
                return new ResponseHandler().Success(entitiesDto, new { NumberOfPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize.Value) });
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<GetBranchEntityDto>>("Something went wrong");
            }
        }

        public async Task<Repositories.Common.Response<GetAreaStatisticsDto>> GetAreaStatistics(int AreaId)
        {
            try
            {
                var entities = await unitOfWork.Repository<Tenant>().FindAllByCriteriaAsync(entity => entity.FedrationBranchId == AreaId && entity.SuperEntityId.HasValue);
                var result = new GetAreaStatisticsDto();
                result.NumberOfEntities = entities.Count;
                result.NumberOfClubs = entities.Count(entity => entity.FedrationEntityTypeId == 2);
                result.NumberOfAcademies = entities.Count(entity => entity.FedrationEntityTypeId == 1);
                result.NumberOfYouthClubs = entities.Count(entity => entity.FedrationEntityTypeId == 6);
                result.NumberOfTrainingCenters = entities.Count(entity => entity.FedrationEntityTypeId == 5);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetAreaStatisticsDto>("Something went wrong");
            }
        }

        public async Task<Repositories.Common.Response<PaginationToReturnDto<SuperEntityToReturnDto>>> GetAllSuperEntitiesAsync(SuperEntityDto dto)
        {
            try
            {

                (int pageSize, int pageNumber) = ValidateOnPagination(dto.PageNumber, dto.PageSize);

                string searchTerm = string.IsNullOrWhiteSpace(dto.SearchTerm) ? "" : dto.SearchTerm.ToLower().Replace(" ", "");

                var result = await fetchService.GetUserBlockedEntitiesAsync();

                Expression<Func<Tenant, bool>> critera;

                if (result.Succeeded && result.Data.Any())
                {

                    critera = st =>

                    st.SuperEntityId == null &&  st.EntityTypeId == 1 &&

                   (string.IsNullOrWhiteSpace(dto.SearchTerm)
                   || st.Name.ToLower().Replace(" ", "").Contains(searchTerm)
                   || (st.Location != null && st.Location.ToLower().Replace(" ", "").Contains(searchTerm))) &&

                   (!dto.EntityTypeId.HasValue || dto.EntityTypeId == st.EntityTypeId)

                   && !result.Data.Contains(st.Id);
                }
                else
                {
                    critera = st =>

                 st.SuperEntityId == null && st.EntityTypeId == 1 &&

                      (string.IsNullOrWhiteSpace(dto.SearchTerm)
                      || st.Name.ToLower().Replace(" ", "").Contains(searchTerm)
                      || (st.Location != null && st.Location.ToLower().Replace(" ", "").Contains(searchTerm))) &&

                      (!dto.EntityTypeId.HasValue || dto.EntityTypeId == st.EntityTypeId);
                }

                Expression<Func<Tenant, SuperEntityToReturnDto>> selector = st => new SuperEntityToReturnDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    Location = st.Location,
                    Image = st.Image,
                    EntityTypeId = st.EntityTypeId
                };

                var superEntitySpec = new GeneralSpecifications<Tenant>(critera,
                pageNumber,
                pageSize
                );

                superEntitySpec.AsNoTracking = true;

                var superEntities = await unitOfWork.Repository<Tenant>().GetAllWithSpecificationsAndProjection(superEntitySpec, selector);

                var tasks = superEntities.Select(async superEntityDto =>
                {
                    if (!string.IsNullOrEmpty(superEntityDto.Image))
                    {
                        superEntityDto.Image = await DocumentSetting.GetPreSignedUrlAsync(superEntityDto.Image);
                    }
                }).ToList();

                await Task.WhenAll(tasks);

                var totalCount = await unitOfWork.Repository<Tenant>().GetCountByCriteriaAsync(critera);

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var paginationToReturnDto = new PaginationToReturnDto<SuperEntityToReturnDto>(totalCount, totalPages, pageSize, pageNumber, superEntities);


                return new ResponseHandler().Success(paginationToReturnDto);
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetAllSuperEntities: {ErrorMessage}", ex.Message);

                return new ResponseHandler().BadRequest<PaginationToReturnDto<SuperEntityToReturnDto>>("Something went wrong");
            }
        }

        public async Task<Repositories.Common.Response<List<object>>> GetEntityTypeAsync(string? searchTerm)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(searchTerm))
                    searchTerm.ToLower().Replace(" ", "");

                Expression<Func<EntityType, bool>> critera = et => string.IsNullOrWhiteSpace(searchTerm) || et.Name.ToLower().Replace(" ", "").Contains(searchTerm);

                var spec = new GeneralSpecifications<EntityType>(critera) { AsNoTracking = true };

                Expression<Func<Tenant, object>> selector = et => (object)new
                {
                    Id = et.EntityTypeId,
                    Name = et.EntityType.Name
                };

                var entityTypes = await unitOfWork.Repository<EntityType>().GetAllWithSpecificationsAndProjection(spec, et => (object)new
                {
                    Id = et.Id,
                    Name = et.Name
                });

                return new ResponseHandler().Success(entityTypes);

            }
            catch (Exception ex)
            {

                Log.Error("Error in GetEntityTypeAsync: {ErrorMessage}", ex.Message);

                return new ResponseHandler().BadRequest<List<object>>("Something went wrong");

            }
        }

        public async Task<Repositories.Common.Response<string>> ReachOutToTenantAsync(ReachOutToTenant dto, string userName, string userEmail)
        {

            try
            {
                var tenant = await unitOfWork.Repository<Tenant>().GetByIdAsync(dto.TenantId);

                if (tenant == null)
                {
                    return new ResponseHandler().NotFound<string>("Tenant not found");
                }

                if (string.IsNullOrWhiteSpace(tenant.Email))
                {
                    return new ResponseHandler().BadRequest<string>("Tenant does not have an email address");
                }

                if (dto.Attachment != null)
                {
                    bool isValid = DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(dto.Attachment);
                    if (!isValid)
                    {
                        return new ResponseHandler().UnprocessableEntity<string>("Attachment is not in a valid format or exceeds the maximum size limit.");
                    }
                }

                var emailBody = ReachOutEmailTemplateBuilder.BuildReachOutEmailBody(dto, userName, userEmail);

                var attachments = dto.Attachment != null
                ? new List<EmailAttachmentDto>
                {
                    await ToAttachmentAsync(dto.Attachment)
                }
                : null;
                var emailMessage = new PushEmail
                {
                    Email = tenant.Email,
                    Subject = dto.Subject,
                    Attachments = attachments,
                    Body = emailBody







                };

                await _publishEndpoint.Publish(emailMessage);

                return new ResponseHandler().Success("Email sent successfully to tenant");
            }
            catch (Exception ex)
            {

                Log.Error("Error in ReachOutToTenantAsync: {ErrorMessage}", ex.Message);

                return new ResponseHandler().Exception<string>("Something went wrong");
            }

        }
        private (int pageSize, int pageNumber) ValidateOnPagination(int pageNumber = 1, int pageSize = 25)
        {

            if (pageSize <= 0 || pageSize > 25)
            {
                pageSize = 25;
            }

            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }
            return (pageSize, pageNumber);

        }

        private static async Task<EmailAttachmentDto> ToAttachmentAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            return new EmailAttachmentDto
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Content = ms.ToArray()
            };
        }
    }
}
