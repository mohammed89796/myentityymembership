using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.GenericRepo;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MyEntityMembershipService.Repositories.Membership.PackageRequestAuthourizationsRepo
{
    public class PackageRequestAuthorizationRepository : GenericRepository<PackageRequestAuthourizations>, IPackageRequestAuthorizationRepository
    {
        private readonly EntityMembershipDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;

        public PackageRequestAuthorizationRepository(EntityMembershipDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(context)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
        }

        //public async Task<List<PackageRequestAuthourizations>> GetAuthorizationsByPackageIdAsync(int packageId)
        //{
        //    return await _context.PackageRequestAuthourizations
        //        .Where(p => p.PackageId == packageId)
        //        //.Where(p => !(p.RequestType.Id == 6 && p.RequestType.Name == "New Membership Request")) // Exclude if Id is 6 and Name is "New Membership Request"
        //        //.Where(p => !(p.RequestType.Id != 6 && p.RequestType.Name == "New Membership Request")) // Exclude others with Name "New Membership Request"

        //        .Include(p => p.RequestType)
        //            .ThenInclude(r => r.Forms)
        //            .ThenInclude(r => r.FormFields)
        //        .Include(p => p.RequestType)
        //            .ThenInclude(r => r.Configuration)
        //            .ThenInclude(r=>r.PackageConfigurationSections)// Include RequestType's Configurations
        //            .ThenInclude(r => r.ConfigurationFields)
        //            .ThenInclude(r => r.FieldType)
        //          .Include(p => p.RequestType)
        //            .ThenInclude(r => r.Configuration)
        //            .ThenInclude(r=>r.ConfigrationFieldResponses.Where(cfr => cfr.PackageId == packageId))
        //        .ToListAsync();
        //}
        public async Task<List<PackageRequestAuthourizations>> GetAuthorizationsByPackageIdAsync(int packageId)
        {
            // Get the MemberTypeIds for the package
            var memberTypeIds = await GetPackageMemberTypeIdsAsync(packageId);

            // Get the MemberType names from the MemberTypeIds
            var memberTypeNames = await GetMemberTypeNamesByIdsAsync(memberTypeIds);

            return await _context.PackageRequestAuthourizations
                .Where(p => p.PackageId == packageId)

                .Include(p => p.RequestType)
                    .ThenInclude(r => r.Forms .Where(f=>f.IsDraft!=true))
                    .ThenInclude(f => f.FormFields)

                .Include(p => p.RequestType)
                    .ThenInclude(r => r.Configuration)
                    .ThenInclude(c => c.PackageConfigurationSections)
                    .ThenInclude(s => s.ConfigurationFields
                        .Where(cf => cf.PackageConfigurationSectionId != 6 || memberTypeNames.Contains(cf.Name))) // Filter fields for PackageConfigurationSectionId.Id == 4
                    .ThenInclude(r => r.FieldType)

                .Include(p => p.RequestType)
                    .ThenInclude(r => r.Configuration)
                    .ThenInclude(c => c.ConfigrationFieldResponses
                        .Where(cfr => cfr.PackageId == packageId)) // Filter responses by PackageId

                .ToListAsync();
        }


        public async Task<List<PackageMemberTypeDto>> GetMemberTypesWithRequestTypesAndForms(int packageId)
        {
            // Step 1: Fetch the member type IDs for the package
            List<int> memberTypeIds = await GetPackageMemberTypeIdsAsync(packageId);

            // Step 2: Fetch the relevant RequestTypeIds associated with the package
            var packageRequestTypeIds = await _context.PackageRequestAuthourizations
                .Where(prt => prt.PackageId == packageId)
                .Where(p => p.RequestType.Id != 6) // Exclude if Id is 6  (New)
                .Select(prt => prt.RequestAuthourizationId)
                .ToListAsync();

            // Step 3: Fetch the FormMemberTypes and related data
            var formMemberTypes = await _context.FormMemberType
                 .Include(fmt => fmt.MemberType)
                 .Include(fmt => fmt.Form)
                     .ThenInclude(f => f.RequestType)
                 .Include(fmt => fmt.Form)
                     .ThenInclude(f => f.PaakageMemberForms.Where(pmf => pmf.PackageId == packageId)) // Filter on PaakageMemberForms
                 .Where(fmt => fmt.Form.IsDraft != true) // Filter non-draft forms
                 .ToListAsync();


            if (formMemberTypes == null)
            {
                return new List<PackageMemberTypeDto>();
            }

            // Step 4: Filter and map the data
            var memberTypesWithRequestTypes = formMemberTypes
                .Where(fmt => fmt.Form != null
            && fmt.Form.MemberTypes != null
            && fmt.Form.MemberTypes.Any(mt => mt.MemberTypeId == fmt.MemberTypeId) // Ensure form contains the relevant member type
            && memberTypeIds.Contains(fmt.MemberTypeId) // Ensure member type is part of the package
            && packageRequestTypeIds.Contains(fmt.Form.RequestTypeId) // Ensure request type is part of the package
                                                                      // Apply conditions for Main Member (Id = 5)
            && (fmt.MemberTypeId == 5 && new[] { 7, 12, 17 }.Contains(fmt.Form.RequestTypeId) // Allow only specific request types for Main Member
                || fmt.MemberTypeId != 5 && !new[] { 7, 17 }.Contains(fmt.Form.RequestTypeId) // Exclude these for non-Main Members
                // Ensure request types 9 and 18 are available only for non-main members
                && (fmt.MemberTypeId != 5 || !new[] { 9, 18 }.Contains(fmt.Form.RequestTypeId)))
    

                ).Select(fmt => new PackageMemberTypeDto
                {
                    Id = fmt.MemberTypeId,
                    Name = fmt.MemberType.Name,
                    RequestTypes = fmt.Form.RequestType != null ? new List<PackageRequestTypeDto>
                    {
                new PackageRequestTypeDto
                {
                    Id = fmt.Form.RequestType.Id,
                    Name = fmt.Form.RequestType.Name,
                    PackageForms = fmt.Form.RequestType.Forms != null
                        ? fmt.Form.RequestType.Forms
                            .Where(f => f.MemberTypes != null
                                && f.MemberTypes.Any(mt => mt.MemberTypeId == fmt.MemberTypeId)
                                && packageRequestTypeIds.Contains(f.RequestTypeId))
                            .Select(f => new PackageFormDto
                            {
                                Id = f.Id,
                                Title = f.Title,
                                Fees = f.Fees,
                                WorkflowSelection = f.WorkflowSelection,
                                PaakageMemberForms = f.PaakageMemberForms
                                    .Select(pmf => new PaakageMemberFormDto
                                    {
                                        FormId = pmf.FormId,
                                        PackageId = pmf.PackageId,
                                        MemberTypeId = pmf.MemberTypeId,
                                        Fees = pmf.Fees,
                                    })
                                    .Where(m => m.MemberTypeId == fmt.MemberTypeId)
                                    .FirstOrDefault()
                            })
                            .ToList()
                        : new List<PackageFormDto>()

                }
                    }
                    : new List<PackageRequestTypeDto>()
                })
                .ToList();

            // Apply distinct logic after the query
            var distinctMemberTypes = memberTypesWithRequestTypes
                .GroupBy(mt => mt.Id)
                .Select(g => new PackageMemberTypeDto
                {
                    Id = g.Key,
                    Name = g.First().Name,
                    RequestTypes = g.SelectMany(mt => mt.RequestTypes)
                                    .GroupBy(rt => rt.Id)
                                    .Select(rtGroup => new PackageRequestTypeDto
                                    {
                                        Id = rtGroup.Key,
                                        Name = rtGroup.First().Name,
                                        PackageForms = rtGroup.SelectMany(rt => rt.PackageForms)
                                                        .GroupBy(f => f.Id)
                                                        .Select(fg => fg.First())
                                                        .ToList()
                                    })
                                    .ToList()
                })
                .ToList();

            return distinctMemberTypes;
        }

        public async Task<List<PaakageMemberForm>> GetPaakageMemberForms(int packageId)
            => await _context.PaakageMemberForms.Where(item => item.PackageId == packageId)
                                                .Include(item => item.Form).ToListAsync();

        public async Task<List<PackageRequestAuthourizations>> GetPackageAuthorizations(int packageId)
            => await _context.PackageRequestAuthourizations
                .Where(p => p.PackageId == packageId)
                .Include(p => p.RequestType)
                    .ThenInclude(r => r.Forms)
                    .ThenInclude(f => f.PaakageMemberForms).ToListAsync();

        public async Task<List<int>> GetPackageMemberTypeIdsAsync(int packageId)
        {
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Make the GET request
                var requestUrl = $"{configuration["ApiSettings:Gateway"]}{configuration["MyEntityServiceUrl:GetPackageById"]}/{packageId}";
                var response = await httpClient.GetAsync(requestUrl);

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
        public async Task<List<string>> GetMemberTypeNamesByIdsAsync(List<int> memberTypeIds)
        {
            // Assuming you have a MemberType table that has an Id and Name field
            return await _context.MemberType
                .Where(mt => memberTypeIds.Contains(mt.Id))
                .Select(mt => mt.Name)
                .ToListAsync();
        }



    }
}
