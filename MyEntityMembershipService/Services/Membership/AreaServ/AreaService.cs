using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.AreaDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.AreaServ
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IFetchService fetchService;
        private readonly IHttpContextAccessor httpContext;

        public AreaService(IUnitOfWork unitOfWork, IMapper mapper, IFetchService fetchService, IHttpContextAccessor httpContext)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.fetchService = fetchService;
            this.httpContext = httpContext;
        }

        public async Task<Response<bool>> AddArea(AddAreaDto AreaDto)
        {
            try
            {
                if (AreaDto.FederationId is null)
                    return new ResponseHandler().BadRequest<bool>("Invalid Tenant Id");
                var area = mapper.Map<Area>(AreaDto);

                // Get List of Belts
                var Belts = await unitOfWork.Repository<DropdownValue>().FindAllByCriteriaAsync(value => value.FieldId == 83);
                var areaPricing = mapper.Map<List<AreaPricing>>(Belts);
                area.AreaPrices = areaPricing;

                // check if Federation Doesn't exist in Membership Tenant table search in Entity Tenant table

                var specs = new GeneralSpecifications<Tenant>(t => t.Id == AreaDto.FederationId)
                {
                    Includes =
                    [
                        Iq => Iq.Include(t => t.Packages).ThenInclude(p => p.PackageMemberForms).ThenInclude(pmf => pmf.Form),

                    ]
                };

                var tenant = await unitOfWork.Repository<Tenant>().GetWithSpecifications(specs);
                if (tenant is null || tenant.Id == 0)
                    return new ResponseHandler().BadRequest<bool>("Invalid Tenant Id");

                var packagesForm = tenant.Packages.Take(1).SelectMany(p => p.PackageMemberForms).Select(f => new { f.FormId , f.Form?.RequestTypeId, f.Form.Fees}).ToList();
                Log.Information("Packages Forms: {@PackagesForm}", packagesForm);

                area.AreaConfigurations = packagesForm.Select(f => new AreaConfiguration
                {
                    FormId = f.FormId,
                    OperationId = f.RequestTypeId.Value,
                    FormFee = f?.Fees ?? 0
                    
                }).ToList();

               
                area.TenantAreas = new List<TenantArea>()
                {
                    new TenantArea (){ TenantId = AreaDto.FederationId.Value }
                };

                await unitOfWork.Repository<Area>().Add(area);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                Log.Error("Couldn't create the area", ex);
                return new ResponseHandler().BadRequest<bool>("Something Went Wrong");
                
            }
        }

        public async Task<Response<List<GetAreasDto>>> GetAllAreas(int FederationId, string? SearchBy, int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                var areasDto = new List<GetAreasDto>();

                var userId = httpContext.HttpContext?.User.GetUserId();
                // FilterationAccording For Admin
                var entityAdmin = await fetchService.GetTenantAdmin(userId); // Should be replaced with FetchService

                var areasId = entityAdmin.TenantAreaAdmins.Select(areaAdmin => areaAdmin.AreaId).ToList();
                var totalCountSpec = new GeneralSpecifications<TenantArea>(item => item.TenantId == FederationId && (!areasId.Any() || areasId.Contains(item.AreaId)) &&
                    (string.IsNullOrEmpty(SearchBy) || item.Area.Name.ToLower().Contains(SearchBy.ToLower())));
                var totalCount = await unitOfWork.Repository<TenantArea>().GetAllWithSpecifications(totalCountSpec);

                GeneralSpecifications<TenantArea> spec;
                

                spec = new GeneralSpecifications<TenantArea>(item => item.TenantId == FederationId 
                && (!areasId.Any() || areasId.Contains(item.AreaId))
                && (string.IsNullOrEmpty(SearchBy) || item.Area.Name.ToLower().Contains(SearchBy.ToLower())),
                            PageIndex, PageSize);
                spec.AddInclude(item => item.Include(item => item.Area).ThenInclude(area => area.Entities));
                spec.SetOrderByDescending(item => item.CreationDate);



                var data = await unitOfWork.Repository<TenantArea>().GetAllWithSpecifications(spec);
                var areas = data.Select(item => item.Area).ToList();
                areasDto = mapper.Map<List<GetAreasDto>>(areas);
                return new ResponseHandler().Success(areasDto, new { NumberOfPages = (int)Math.Ceiling(((double)totalCount.Count / PageSize)) });
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<GetAreasDto>>("Something Went Wrong");
            }
        }

        public async Task<Response<AreaWithAccountsDto>> GetAreaWithBankAccounts(int AreaId)
        {
            try
            {
                var spec = new GeneralSpecifications<Area>(a => a.Id == AreaId);
                spec.Includes = new List<Func<IQueryable<Area>, IQueryable<Area>>>
                {
                    a => a.Include(area => area.BankAccounts)
                };
                var area = await unitOfWork.Repository<Area>().GetWithSpecifications(spec);
                var result = mapper.Map<AreaWithAccountsDto>(area);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AreaWithAccountsDto>("Something Went Wrong");
            }
        }

        public async Task<Response<bool>> UpdateArea(AreaWithAccountsDto areaDto)
        {
            try
            {
                var spec = new GeneralSpecifications<Area>(a => a.Id == areaDto.Id);
                spec.AddInclude(a => a.Include(a => a.BankAccounts));
                var area = await unitOfWork.Repository<Area>().GetWithSpecifications(spec);
                if (area is null)
                    return new ResponseHandler().BadRequest<bool>("There is No Area With that Id");
                area.Name = areaDto.Name;
                if (areaDto.BankAccounts is not null)
                {
                    //For Delete accounts that exist in the area Accounts but doesn't exist in the input
                    var accountsToRemove = area.BankAccounts.Where(account => !areaDto.BankAccounts.Any(dtoAccount => dtoAccount.Id == account.Id)).ToList();
                    foreach (var account in accountsToRemove)
                        area.BankAccounts.Remove(account);

                    // For Update account if exist Or Add it if it doesn't
                    foreach (var account in areaDto.BankAccounts)
                    {
                        var existing = area.BankAccounts.FirstOrDefault(a => a.Id == account.Id);
                        if (existing is not null)
                        {
                            existing.AccountNumber = account.AccountNumber;
                            existing.BankName = account.BankName;
                        }
                        else
                        {
                            var newBankAccount = mapper.Map<BankAccount>(account);
                            area.BankAccounts.Add(newBankAccount);
                        }
                    }
                }
                else
                    foreach (var account in area.BankAccounts)
                        area.BankAccounts.Remove(account);

                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something Went Wrong");
            }
        }

        public async Task<Response<List<AreaPricingDto>>> GetAllAreaPricing(GetAllAreaPricingParametersDto parameters)
        {
            try
            {
                var totalCount = await unitOfWork.Repository<AreaPricing>().GetCountByCriteriaAsync(pricing => pricing.AreaId == parameters.AreaId &&
                (string.IsNullOrEmpty(parameters.SearchBy) || pricing.BeltName.ToLower().Contains(parameters.SearchBy.ToLower())));
                var spec = new GeneralSpecifications<AreaPricing>(pricing => pricing.AreaId == parameters.AreaId && 
                (string.IsNullOrEmpty(parameters.SearchBy) || pricing.BeltName.ToLower().Contains(parameters.SearchBy.ToLower()) ),
                parameters.PageIndex, parameters.PageSize);
                var pricing = await unitOfWork.Repository<AreaPricing>().GetAllWithSpecifications(spec);
                var areaPricingDo = mapper.Map<List<AreaPricingDto>>(pricing);
                return new ResponseHandler().Success(areaPricingDo, new { NumberOfPage = (int)Math.Ceiling(((double)totalCount / parameters.PageSize)) });
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<AreaPricingDto>>("Something Went Wrong");
            }
        }

        public async Task<Response<bool>> UpdateAreaPricing(UpdateAreaPricingDto areaPricingDto)
        {
            try
            {
                var areaPricing = await unitOfWork.Repository<AreaPricing>().GetByIdAsync(areaPricingDto.Id);
                if (areaPricing is null)
                    return new ResponseHandler().BadRequest<bool>("There is no Area Pricing With this Id");
                mapper.Map(areaPricingDto, areaPricing);
                unitOfWork.Repository<AreaPricing>().Update(areaPricing);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something Went Wrong");
            }
        }

        public async Task<Response<bool>> AddAreaConfiguration(AddAreaConfigurationDto areaConfigurationDto)
        {
            try
            {
                var form = await unitOfWork.Repository<Form>().GetByIdAsync(areaConfigurationDto.FormId);
                if (form is null)
                    return new ResponseHandler().BadRequest<bool>("There is No Form With this Id");
                var area = await unitOfWork.Repository<Area>().GetByIdAsync(areaConfigurationDto.AreaId);
                if (area is null)
                    return new ResponseHandler().BadRequest<bool>("There is No Area with this Id");
                var config = await unitOfWork.Repository<AreaConfiguration>()
                    .FindByCriteriaAsync(config => config.AreaId == areaConfigurationDto.AreaId && config.OperationId == areaConfigurationDto.OperationId && config.FormId == areaConfigurationDto.FormId);
                if (config is not null)
                    return new ResponseHandler().BadRequest<bool>("This Branch Already Have Configuration For this Operation");
                var areaConfiguration = mapper.Map<AreaConfiguration>(areaConfigurationDto);
                areaConfiguration.FormFee = form.Fees.Value;
                await unitOfWork.Repository<AreaConfiguration>().Add(areaConfiguration);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something Went Wrong");
            }
        }

        public async Task<Response<List<AreaConfigurationDto>>> GetAreaConfigurations(int AreaId)
        {
            try
            {
                var spec = new GeneralSpecifications<AreaConfiguration>(config => config.AreaId == AreaId);
                spec.AddInclude(config => config.Include(config => config.Form));
                var areaConfigurations = await unitOfWork.Repository<AreaConfiguration>().GetAllWithSpecifications(spec);
                var result = mapper.Map<List<AreaConfigurationDto>>(areaConfigurations);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<AreaConfigurationDto>>("Something Went Wrong");
            }
        }

        public async Task<Response<bool>> UpdateAreaConfiguration(UpdateAreaConfigurationDto areaConfigurationDto)
        {
            try
            {
                var config = await unitOfWork.Repository<AreaConfiguration>().GetByIdAsync(areaConfigurationDto.Id);
                if (config is null)
                    return new ResponseHandler().BadRequest<bool>("There is No Configuration With this Id");
                var form = await unitOfWork.Repository<Form>().GetByIdAsync(areaConfigurationDto.FormId);
                if (form is null)
                    return new ResponseHandler().BadRequest<bool>("There is No Form With this Id");

                mapper.Map(areaConfigurationDto, config);
                config.FormFee = form.Fees.Value;
                unitOfWork.Repository<AreaConfiguration>().Update(config);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something Went Wrong");
            }
        }

        public async Task<Response<bool>> DeleteAreaConfiguration(int Id)
        {
            try
            {
                var config = await unitOfWork.Repository<AreaConfiguration>().GetByIdAsync(Id);
                if (config is null)
                    return new ResponseHandler().BadRequest<bool>("There is No Configuration with this Id");
                unitOfWork.Repository<AreaConfiguration>().Delete(config);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something Went Wrong");
            }
        }

        public async Task<Response<bool>> AddAreaBankAccount(AddAreaBankAccountDto parameters)
        {
            try
            {
                var area = await unitOfWork.Repository<Area>().GetByIdAsync(parameters.AreaId);
                if (area is null)
                    return new ResponseHandler().BadRequest<bool>("There is no Branch With this Id");
                var account = mapper.Map<BankAccount>(parameters.BankAccount);
                account.AreaId = parameters.AreaId;
                await unitOfWork.Repository<BankAccount>().Add(account);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something Went Wrong");
            }
        }

        public async Task<Response<List<GetAreasDto>>> GetAllFederationAreas(int FederationId, string userId)
        {
            try
            {
                var areasDto = new List<GetAreasDto>();
                // FilterationAccording For Admin
                var entityAdmin = await fetchService.GetTenantAdmin(userId); // Should be replaced with FetchService

                //// if entity for user is not Federation 
                //if(entityAdmin.Tenant.SuperEntityId is not null)
                //{

                //    var entityAdminArea = entityAdmin.TenantAreaAdmins.FirstOrDefault();
                //    if (entityAdminArea is not null)
                //    {
                //        var area = new List<Area>() { entityAdminArea.Area };
                //        areasDto = mapper.Map<List<GetAreasDto>>(area);
                //        return new ResponseHandler().Success(areasDto);
                //    }
                //}

                GeneralSpecifications<TenantArea> spec;
                var areasId = entityAdmin.TenantAreaAdmins.Select(areaAdmin => areaAdmin.AreaId).ToList();


                spec = new GeneralSpecifications<TenantArea>(item => item.TenantId == FederationId && (!areasId.Any() || areasId.Contains(item.Id)));
                spec.AddInclude(item => item.Include(item => item.Area).ThenInclude(area => area.Entities));



                var data = await unitOfWork.Repository<TenantArea>().GetAllWithSpecifications(spec);
                var areas = data.Select(item => item.Area).ToList();
                areasDto = mapper.Map<List<GetAreasDto>>(areas);
                return new ResponseHandler().Success(areasDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<GetAreasDto>>("Something Went Wrong");
            }
        }
        public async Task<Response<List<GetAreasDto>>> GetAllFederationAreasByEntityId(int entityId)
        {
            try
            {
                var areasDto = new List<GetAreasDto>();
               
                GeneralSpecifications<TenantArea> spec;
                var entity = await unitOfWork.Repository<Tenant>().GetByIdAsync(entityId);
                if(entity.SuperEntityId == null || entity.SuperEntityId == 0)
                {
                    return new ResponseHandler().BadRequest<List<GetAreasDto>>("Something Went Wrong");

                }
                var FederationId = entity.SuperEntityId;

                spec = new GeneralSpecifications<TenantArea>(item => item.TenantId == FederationId);
                spec.AddInclude(item => item.Include(item => item.Area).ThenInclude(area => area.Entities));
                

                var data = await unitOfWork.Repository<TenantArea>().GetAllWithSpecifications(spec);
                var areas = data.Select(item => item.Area).ToList();
                areasDto = mapper.Map<List<GetAreasDto>>(areas);
                return new ResponseHandler().Success(areasDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<GetAreasDto>>("Something Went Wrong");
            }
        }

        public async Task<Response<List<GetBranchEntityDto>>> GetEntitiesByArea(GetEntitiesByAreaParametersDto parameters)
        {
            try
            {
                var totalCount = await unitOfWork.Repository<Tenant>().GetCountByCriteriaAsync(entity => entity.AreaId == parameters.AreaId && entity.SuperEntityId == parameters.FederationId);
                // if they doesn't send pagination retieve all in first page  for drop down 
                if (!parameters.PageIndex.HasValue && !parameters.PageSize.HasValue)
                {
                    parameters.PageIndex = 1;
                    parameters.PageSize = totalCount;
                }
                var spec = new GeneralSpecifications<Tenant>(entity => entity.AreaId == parameters.AreaId && entity.SuperEntityId == parameters.FederationId &&
                                                            (!parameters.EntityTypeId.HasValue || entity.FedrationEntityTypeId == parameters.EntityTypeId) &&
                                                            (string.IsNullOrEmpty(parameters.EntityName) || entity.Name.ToLower().Contains(parameters.EntityName.ToLower())),
                                                            parameters.PageIndex.Value, parameters.PageSize.Value);
                spec.AddInclude(entity => entity.Include(entity => entity.FedrationEntityType));
                spec.SetOrderByDescending(entity => entity.CreationDate);
                var entities = await unitOfWork.Repository<Tenant>().GetAllWithSpecifications(spec);
                var entitiesDto = mapper.Map<List<GetBranchEntityDto>>(entities);
                foreach (var entity in entitiesDto)
                {
                    entity.NumberOfMembers = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetCountByCriteriaAsync(m => m.TenantId == entity.Id);
                }
                return new ResponseHandler().Success(entitiesDto, new { NumberOfPages = (int)Math.Ceiling(((double)totalCount / parameters.PageSize.Value)) });
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<GetBranchEntityDto>>("Something went wrong");
            }
        }
        public async Task<Response<List<GetBranchEntityDto>>> GetEntitiesByAreaWithoutPagination(int areaId)
        {
            try
            { 
                int.TryParse(httpContext.HttpContext.User.GetTenantId(), out  var tenantId);
                var spec = new GeneralSpecifications<Tenant>(entity => entity.AreaId == areaId && entity.Id != tenantId);
                spec.SetOrderByDescending(entity => entity.CreationDate);
                var entities = await unitOfWork.Repository<Tenant>().GetAllWithSpecifications(spec);
                var entitiesDto = mapper.Map<List<GetBranchEntityDto>>(entities);
              
                return new ResponseHandler().Success(entitiesDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<GetBranchEntityDto>>("Something went wrong");
            }
        }

    }
}
