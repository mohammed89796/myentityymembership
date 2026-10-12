using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.TempRegisterResponseDtos;
using MyEntityMembershipService.DTOs.Membership.TenantDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Newtonsoft.Json;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.FetchService
{

    public class FetchService : IFetchService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly string _gatewayUrl;
        private readonly string _userEndPointUrl;
        private readonly string _tenantEndPointUrl;
        private readonly string _tenantAdminEndpointUrl;
        private readonly string _tempRegisterUserUrl;
        private readonly string _competitionEndPointUrl;
        private readonly string _bulkRegisterUsersUrl;
        private readonly string _UpdateTempRegisterUserUrl;
        private readonly string _DeleteTempRegisterUserUrl;
        private readonly string _GetBlockedEntitiesUrl;
        private readonly string _IsBlockedEntityUrl;

        public FetchService(IUnitOfWork unitOfWork, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;

            _httpClient = clientFactory.CreateClient("ResilientClientWithoutTimeOut");
            _gatewayUrl = configuration["ApiSettings:Gateway"] ?? throw new ArgumentNullException(nameof(configuration), "Gateway URL is not configured.");

            _tenantEndPointUrl = configuration["MyEntity:GetTenantData"] ?? throw new ArgumentNullException(nameof(configuration), "Gateway URL is not configured.");
            _tenantAdminEndpointUrl = configuration["MyEntity:GetTenantAdminData"] ?? throw new ArgumentNullException(nameof(configuration), "Gateway URL is not configured.");
            _tempRegisterUserUrl = configuration["Authorization:TempRegisterUser"] ?? throw new ArgumentNullException(nameof(configuration), "TempRegisterUser URL is not configured.");
            _bulkRegisterUsersUrl = configuration["Authorization:BulkRegisterUsers"] ?? throw new ArgumentNullException(nameof(configuration), "BulkRegisterUsersUrl URL is not configured.");
            _UpdateTempRegisterUserUrl = configuration["Authorization:UpdateTempRegisterUser"] ?? throw new ArgumentNullException(nameof(configuration), "BulkRegisterUsersUrl URL is not configured.");
            _DeleteTempRegisterUserUrl = configuration["Authorization:DeleteTempRegisterUser"] ?? throw new ArgumentNullException(nameof(configuration), "DeleteUsersUrl URL is not configured.");
            _GetBlockedEntitiesUrl = configuration["NewsFeed:GetBlockedEntities"] ?? throw new ArgumentNullException(nameof(configuration), "_GetBlockedEntitiesUrl URL is not configured.");
            _IsBlockedEntityUrl = configuration["NewsFeed:IsEntityBlocked"] ?? throw new ArgumentNullException(nameof(configuration), "_IsEntityBlocked URL is not configured.");


        }

        public async Task<TenantAdmin> GetTenantAdmin(string userId)
        {

            try
            {
                var specs = new GeneralSpecifications<TenantAdmin>(
                entity => entity.UserId == userId)
                {
                    Includes = new List<Func<IQueryable<TenantAdmin>, IQueryable<TenantAdmin>>>
                           {
                               e => e.Include(a => a.TenantAreaAdmins)
                                        .ThenInclude(ta => ta.Area)
                                     .Include(a => a.Tenant),
                           }
                };


                var tenantAdminData = await _unitOfWork.Repository<TenantAdmin>().GetWithSpecifications(specs);
                if (tenantAdminData is not null)
                {
                    return tenantAdminData;

                }

                var url = $"{_gatewayUrl}{_tenantAdminEndpointUrl}";


                var requestData = await _httpClient.PutAsJson<object>(url, null!);
                var requestContent = await requestData.Content.ReadAsStringAsync();
                var request = JsonConvert.DeserializeObject<Response<TenantAdminDto>>(requestContent);

                if (request == null)
                {
                    Log.Error("admin data not found for adminId: {userId}", userId);
                    return new TenantAdmin();
                }

                var tenant = await GetTenantEntityAsync(request.Data.TenantId);
                var area = await _unitOfWork.Repository<Area>().FindAsync(x => x.Id == request.Data.BranchId);
                var tenantAdmin = new TenantAdmin();
                if (area is not null)
                    tenantAdmin = new TenantAdmin
                    {
                        Email = request.Data.Email ?? string.Empty,

                        FirstName = request.Data.FirstName ?? string.Empty,
                        LastName = request.Data.LastName ?? string.Empty,
                        PhoneNumber = request.Data.PhoneNumber ?? string.Empty,
                        UserId = request.Data.UserId ?? string.Empty,
                        TenantId = request.Data.TenantId,
                        TenantAreaAdmins = [new TenantAdminArea { AreaId = request.Data.BranchId }],
                    };
                else
                    tenantAdmin = new TenantAdmin
                    {
                        Email = request.Data.Email ?? string.Empty,
                        FirstName = request.Data.FirstName ?? string.Empty,
                        LastName = request.Data.LastName ?? string.Empty,
                        PhoneNumber = request.Data.PhoneNumber ?? string.Empty,
                        UserId = request.Data.UserId ?? string.Empty,
                        TenantId = request.Data.TenantId,
                    };



                await _unitOfWork.Repository<TenantAdmin>().Add(tenantAdmin);


                await _unitOfWork.Save();

                tenantAdmin.Tenant = tenant;
                if (tenantAdmin.TenantAreaAdmins.Any())
                    tenantAdmin.TenantAreaAdmins.FirstOrDefault().Area = area;

                return tenantAdmin;

            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "HTTP request error occurred while getting entity data for admin data not found for adminId: {userId}", userId);
                return new TenantAdmin();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting entity data for admin data not found for adminId: {userId}", userId);
                return new TenantAdmin();
            }



        }

        public async Task<Tenant> GetTenantEntityAsync(int entityId)
        {
            try
            {
                var entityData = await _unitOfWork.Repository<Tenant>().FindByCriteriaAsync(entity => entity.Id == entityId);

                if (entityData is not null)
                {
                    return entityData;
                }
                var url = $"{_gatewayUrl}{_tenantEndPointUrl}?tanantId={entityId}";


                var request = await _httpClient.GetFromJsonAsync<Response<TenantDetailsDto>>(url);

                if (request == null)
                {
                    Log.Error("Entity data not found for entityId: {entityId}", entityId);
                    return new Tenant();
                }


                var entity = new Tenant
                {
                    Id = request.Data.Id,
                    Image = request.Data.Image ?? string.Empty,
                    Name = request.Data.Name ?? string.Empty,
                    EntityTypeId = request.Data?.EntityTypeId,
                    FedrationEntityTypeId = request.Data?.FedrationEntityTypeId,
                    AreaId = request.Data?.FedrationBranchId,
                };

                if (request.Data?.SuperEntity is not null)
                {
                    var fedration = await _unitOfWork.Repository<Tenant>().FindByCriteriaAsync(entity => entity.Id == request.Data.SuperEntity.Id);

                    if (fedration is null)
                        entity.SuperEntity = new Tenant
                        {
                            Id = request.Data.SuperEntity.Id,
                            Image = request.Data.SuperEntity.Image ?? string.Empty,
                            Name = request.Data.SuperEntity.Name ?? string.Empty,
                            EntityTypeId = request.Data.SuperEntity?.EntityTypeId,
                            FedrationEntityTypeId = request.Data.SuperEntity?.FedrationEntityTypeId
                        };

                    else
                    {
                        var superTenant = await GetTenantEntityAsync(request.Data.SuperEntity.Id);

                        entity.SuperEntityId = superTenant.Id;

                    }




                }


                await _unitOfWork.Repository<Tenant>().Add(entity);
                await _unitOfWork.Save();
                return entity;
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "HTTP request error occurred while getting entity data for entityId: {entityId}", entityId);
                return new Tenant();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting entity data for userId: {entityId}", entityId);
                return new Tenant();
            }
        }

        public async Task<Response<string>> TempRegisterUser(TempRegisterRequestDto tempRegisterRequestDto)
        {

            try
            {
                var url = $"{_gatewayUrl}{_tempRegisterUserUrl}";
                var requestData = await _httpClient.PostAsJson<TempRegisterRequestDto, Response<TempRegisterResponseDto>>(url, tempRegisterRequestDto);
                if (!requestData.Succeeded)
                    return new ResponseHandler().BadRequest<string>(requestData.Message);

                return new ResponseHandler().Success(requestData.Data.UserId);


            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }



        }
        public async Task<Response<string>> DeleteUsers(List<string> usersIds)
        {

            try
            {
                var url = $"{_gatewayUrl}{_DeleteTempRegisterUserUrl}";
                var requestData = await _httpClient.PostAsJson<List<string>, Response<string>>(url, usersIds);
                if (!requestData.Succeeded)
                    return new ResponseHandler().BadRequest<string>(requestData.Message);

                return new ResponseHandler().Success(requestData.Message);


            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }



        }

        public async Task<Response<List<TempRegisterResponseDto>>> BulkRegisterUsersAsync(List<TempRegisterRequestDto> users)
        {
            try
            {
                var url = $"{_gatewayUrl}{_bulkRegisterUsersUrl}";

                var requestData = await _httpClient.PostAsJson<List<TempRegisterRequestDto>, Response<List<TempRegisterResponseDto>>>(url, users);

                if (!requestData.Succeeded)
                    return new ResponseHandler().BadRequest<List<TempRegisterResponseDto>>(requestData.Message);


                return new ResponseHandler().Success(requestData.Data);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<TempRegisterResponseDto>>(ex.Message);
            }
        }

        public async Task<Response<UpdateTempUserRequestDto>> UpdateempRegisterUser(UpdateTempUserRequestDto tempRegisterRequestDto)
        {

            try
            {
                var url = $"{_gatewayUrl}{_UpdateTempRegisterUserUrl}";
                var requestData = await _httpClient.PutAsJson<UpdateTempUserRequestDto, Response<UpdateTempUserRequestDto>>(url, tempRegisterRequestDto);
                if (!requestData.Succeeded)
                    return new ResponseHandler().BadRequest<UpdateTempUserRequestDto>(requestData.Message);

                return new ResponseHandler().Success(requestData.Data);


            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<UpdateTempUserRequestDto>(ex.Message);
            }



        }

        public async Task<Response<List<int>>> GetUserBlockedEntitiesAsync()
        {
            try
            {

                var url = $"{_gatewayUrl}{_GetBlockedEntitiesUrl}";
                var requestData = await _httpClient.GetFromJsonAsync<Response<List<int>>>(url);
                if (!requestData.Succeeded)
                    return new ResponseHandler().BadRequest<List<int>>(requestData.Message);

                return new ResponseHandler().Success(requestData.Data);




            }
            catch (Exception ex)
            {

                Log.Error(ex.Message);

                return new ResponseHandler().BadRequest<List<int>>("something went wrong");
            }
        }

        public async Task<Response<bool>> EntityBlockedAsync(int entityId)
        {
            try
            {

                var url = $"{_gatewayUrl}{_IsBlockedEntityUrl}?entityId={entityId}";
                var requestData = await _httpClient.GetFromJsonAsync<Response<bool>>(url);
                if (!requestData.Succeeded)
                    return new ResponseHandler().BadRequest<bool>(requestData.Message);

                return new ResponseHandler().Success(requestData.Data);




            }
            catch (Exception ex)
            {

                Log.Error(ex.Message);

                return new ResponseHandler().BadRequest<bool>("something went wrong");
            }
        }
    }

}
