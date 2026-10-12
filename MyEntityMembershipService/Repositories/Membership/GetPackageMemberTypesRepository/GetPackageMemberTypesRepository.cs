using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.GetPackageMemberTypesDtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.GenericRepo;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MyEntityMembershipService.Repositories.Membership.GetPackageMemberTypesRepository
{
    public class GetPackageMemberTypesRepository : GenericRepository<MemberType>, IGetPackageMemberTypesRepository
    {
        private readonly EntityMembershipDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;


        public GetPackageMemberTypesRepository(EntityMembershipDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(context)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
        }
        public async Task<List<int>> GetPackageMemberTypeIdsAsync(int packageId)
        {
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            using (var httpClient = new HttpClient())
            {

                var client = new HttpClient();
                var url = $"{configuration["ApiSettings:Gateway"]}/EMembership/1/Package/GetPackageById/{packageId}";
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Make the GET request
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();


                    // Deserialize the JSON response to a PackageDto object
                    var result = JsonConvert.DeserializeObject<Response<PackageDto>>(jsonResponse);

                    var packageDto = result?.Data;

                    // Check if the deserialized object is null or its properties are null
                    if (packageDto == null || packageDto.SubMemberCategories == null)
                    {
                        throw new Exception("Deserialization failed or the API returned unexpected data.");
                    }

                    // Return the list of MemberTypeIds from the SubMembers collection
                    return packageDto.SubMemberCategories.Select(sm => sm.MemberTypeId).ToList();
                }
                else
                {
                    // Handle unsuccessful response
                    throw new Exception($"Failed to retrieve package data. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }
            }
        }
        public async Task<List<GetPackageMemberTypesDto>> GetMemberTypeNamesByIdsAsync(List<int> memberTypeIds)
        {
            // Assuming you have a MemberType table that has an Id and Name field
            return await _context.MemberType
                .Where(mt => memberTypeIds.Contains(mt.Id))
                .Select(mt => new GetPackageMemberTypesDto{MemberId = mt.Id, MemberName = mt.Name })
                .ToListAsync();

        }

           

    }
}
