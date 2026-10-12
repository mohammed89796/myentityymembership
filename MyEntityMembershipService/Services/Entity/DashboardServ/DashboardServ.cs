using AutoMapper;
using MyEntityMembershipService.DTOs.Entity.DashboardDtos;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Entity.FacilityService;
using MyEntityMembershipService.Services.Entity.TenantService;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MyEntityMembershipService.Services.Entity.DashboardServ
{
    public class DashboardServ : IDashboardServ
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ITenantService _tenantService;
        private readonly IFacilityService _facilityService;

        public DashboardServ(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ITenantService tenantService, IFacilityService facilityService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _tenantService = tenantService;
            _facilityService = facilityService;
        }

        public async Task<Response<DashboardTenentDetails>> GetDashboardTenatSummary(int? tenantId)
        {
            try
            {

                if(tenantId is null)
                {
                    return new ResponseHandler().BadRequest<DashboardTenentDetails>("tenantId is required");
                }

                var tenant = await _tenantService.GetTenantByIdAsync(tenantId.Value);
                if (tenant == null)
                {
                    return new ResponseHandler().BadRequest<DashboardTenentDetails>("tenant not found");

                }
                var tenantDto = _mapper.Map<TenantDto, AddTenantDto>(tenant.Data);

                var facilities = await _facilityService.GetTotalNumberOfFacilitiesByTenantId(tenantId.Value);

                var memberships = await GetTotalNoumberOfMembershipsAsync(tenantId.Value);
                if (memberships == null)
                {
                    return new ResponseHandler().BadRequest<DashboardTenentDetails>("memberships not found");

                }
                var programs = await GetAllProgramsCount(tenantId.Value);
                if (programs == null)
                {
                    return new ResponseHandler().BadRequest<DashboardTenentDetails>("programs not found");

                }


                var dashboardTenentDetails = new DashboardTenentDetails
                {
                    Tenant = tenantDto,
                    TotalNoumberOfFacilities = facilities,
                    TotalNoumberOfMemberships = memberships.Data,
                    TotalNoumberOfPrograms = programs.Data

                };

                dashboardTenentDetails.Tenant.Image = await DocumentSetting.GetPreSignedUrlAsync(tenantDto.Image);



                // Return the response
                return new ResponseHandler().Success(dashboardTenentDetails);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return a bad request response
                return new ResponseHandler().BadRequest<DashboardTenentDetails>("Something went wrong");
            }
        }

        private async Task<Response<int>> GetTotalNoumberOfMembershipsAsync(int tenantId)
        {
            try
            {
                var httpClient = new HttpClient();
                var apiUrl = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MyMemembershipServiceUrl:GeMembershipsByTenantId"]}/{tenantId}";

                // Fetch Bearer token
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send GET request to fetch venue details
                var response = await httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseHandler().NotFound<int>("Invalid tenant or date");
                }

                // Deserialize the content into a strongly-typed ApiResponse containing a List of MembershipDto
                var contentResult = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Response<List<MembershipDto>>>(contentResult);

                // Check if deserialized content is null or if the Data is null/empty
                if (apiResponse == null || apiResponse.Data == null || !apiResponse.Data.Any())
                {
                    return new ResponseHandler().NotFound<int>("Membership data not found in the response");
                }

                // Get the count of memberships
                int membershipCount = apiResponse.Data.Count;

                return new ResponseHandler().Success(membershipCount);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<int>("Something went wrong");
            }
        }
        private async Task<Response<int>> GetAllProgramsCount(int tenantId)
        {
            try
            {
                var httpClient = new HttpClient();
                var apiUrl = $"{_configuration["ApiSettings:Gateway"]}{_configuration["MyProgramServiceUrl:GetAllProgramsCount"]}/{tenantId}";

                // Fetch Bearer token
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send GET request to fetch venue details
                var response = await httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseHandler().NotFound<int>("Invalid tenant or date");
                }

                // Deserialize the content into a strongly-typed ApiResponse containing a List of MembershipDto
                var contentResult = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<Response<int>>(contentResult);

                // Check if deserialized content is null or if the Data is null/empty
                if (apiResponse == null || apiResponse.Data == null)
                {
                    return new ResponseHandler().NotFound<int>("Program data not found in the response");
                }

                // Get the count of memberships
                int membershipCount = apiResponse.Data;

                return new ResponseHandler().Success(membershipCount);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<int>("Something went wrong");
            }
        }



    }
}
