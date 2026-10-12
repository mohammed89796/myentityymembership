using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.PackageRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;


namespace MyEntityMembershipService.Services.Entity.PackageService
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IPackageRepository packageRepository;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper, IPackageRepository packageRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.packageRepository = packageRepository;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<PackageDto>> AddPackage(AddPackageDto addPackageDto, IFormFile image)
        {
            using (var transaction = await unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var mainMember = new SubMemberCategoryDto
                    {
                        MemberTypeId = 5, //FIXME: Change this static ID
                        EnglishTitle = "MainMember",
                        ArabicTitle = "العضو الرئيسي",
                        Price = 0,
                    };
                    addPackageDto.SubMemberCategories.Add(mainMember);
                    // Map and save the package
                    var package = mapper.Map<Package>(addPackageDto);
                    package.Image = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", image);
                    package.PackageType=PackageType.Public;
                    package.Branches.Clear();

                    if (package.SubMembersLimit == 0)
                    {
                        package.SubMemberCategories.Clear();
                    }


                    await unitOfWork.Repository<Package>().Add(package);
                    await unitOfWork.Save();

                    // Add branches, facilities, and venues
                    if (addPackageDto.Branches != null && addPackageDto.Branches.Any())
                    {
                        foreach (var branchDto in addPackageDto.Branches)
                        {
                            var branchExists = await unitOfWork.Repository<Branch>().GetByIdAsync(branchDto.BranchId);
                            if (branchExists == null)
                            {
                                throw new Exception($"Branch with Id {branchDto.BranchId} does not exist.");
                            }

                            var accessedBranch = new AccessedBranches
                            {
                                PackageId = package.Id,
                                BranchId = branchDto.BranchId
                            };
                            package.Branches.Add(accessedBranch);

                            foreach (var facilityDto in branchDto.FacilityDtos)
                            {
                                var accessedBranchFacility = new AccessedBranchesFacilities
                                {
                                    PackageId = package.Id,
                                    BranchId = branchDto.BranchId,
                                    FacilityId = facilityDto.FacilityId
                                };
                                package.Facilities.Add(accessedBranchFacility);

                                foreach (var venueDto in facilityDto.VenueDtos)
                                {
                                    var accessedFacilityVenue = new AccessedFacilitiesVenues
                                    {
                                        PackageId = package.Id,
                                        BranchId = branchDto.BranchId,
                                        FacilityId = facilityDto.FacilityId,
                                        VenueId = venueDto.VenueId
                                    };
                                    package.Venues.Add(accessedFacilityVenue);
                                }
                            }
                        }
                        await unitOfWork.Save();

                    }


                    // Prepare the list of PackageRequestAuthorizations with the saved PackageId
                    var packageRequestAuthorizationDtos = addPackageDto.PackageAuthourizations
                        .Select(auth => new PackageAuthourizationsDto
                        {
                            PackageId = package.Id, // Use the saved PackageId
                            RequestAuthourizationId = auth.RequestAuthourizationId,
                        })
                        .ToList();
                    var packageNewRequestAuthorizationDtos = new PackageAuthourizationsDto
                    {
                        PackageId = package.Id, // 
                        RequestAuthourizationId = 6,
                    };
                    packageRequestAuthorizationDtos.Add(packageNewRequestAuthorizationDtos);
                    // Make the API call to add PackageRequestAuthorizations
                    var apiCallResponse = await PostPackageRequestAuthorizationsAsync(packageRequestAuthorizationDtos);

                    if (!apiCallResponse.IsSuccessStatusCode)
                    {
                        // API call failed, rollback the transaction
                        await transaction.RollbackAsync();
                        return new ResponseHandler().BadRequest<PackageDto>("Failed to add package request authorizations. Rolled back transaction.");
                    }

                    await transaction.CommitAsync();

                    var packageDto = mapper.Map<PackageDto>(package);
                    return new ResponseHandler().Success(packageDto);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseHandler().BadRequest<PackageDto>("Something went wrong");
                }
            }
        }

        public async Task<Response<List<PackageDto>>> GetAllPackageByTenantId(int? tenantId)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                var packages = await packageRepository.GetAllPackageByTenantId(tenantId);
                var result = mapper.Map<List<PackageDto>>(packages);
                // Get active package ids subscribed by user in same tenant
                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(x =>
                    x.TenantId == tenantId &&
                    x.UserId == userId &&
                    x.PackageId != null &&
                    x.MembershipStatus == MembershipStatus.Active
                );

                var memberships = await unitOfWork
                    .Repository<Data.Membership.Entities.Membership>()
                    .GetAllWithSpecifications(spec);

                var subscribedPackageIds = memberships
                    .Select(x => x.PackageId.Value)
                    .Distinct()
                    .ToList();
                var tasks = result.Select(async dto =>
                {
                    dto.IsSubscribed = subscribedPackageIds.Contains(dto.Id);

                    dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);

                    var branchTasks = dto.AcssedBranches?.Select(async branchDto =>
                    {
                        branchDto.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDto.BranchImageURL);
                    }) ?? Enumerable.Empty<Task>();


                    var facilityTasks = dto.Facilities?.Select(async facilityDto =>
                    {
                        facilityDto.Image = await DocumentSetting.GetPreSignedUrlAsync(facilityDto.Image);
                    }) ?? Enumerable.Empty<Task>();



                    var venueTasks = dto.Venues?.Select(async venueDto =>
                    {
                        venueDto.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(venueDto.VenueImageURL);
                    }) ?? Enumerable.Empty<Task>();

                    await Task.WhenAll(branchTasks
                            .Concat(facilityTasks)
                            .Concat(venueTasks));
                }).ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<PackageDto>>("Something went wrong");
            }
        }
        public async Task<Response<List<PackageDto>>> GetSubscribedPackagesByTenantId(int tenatId)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                // Get active subscriptions
                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(x =>
                    x.TenantId == tenatId &&
                    x.UserId == userId &&
                    x.PackageId != null &&
                    x.MembershipStatus == MembershipStatus.Active
                );

                var memberships = await unitOfWork
                    .Repository<Data.Membership.Entities.Membership>()
                    .GetAllWithSpecifications(spec);

                var subscribedPackageIds = memberships
                    .Select(x => x.PackageId.Value)
                    .Distinct()
                    .ToList();

                if (!subscribedPackageIds.Any())
                    return new ResponseHandler().Success(new List<PackageDto>());

                // Get only subscribed packages
                var packages = await packageRepository.GetAllPackageByTenantId(tenatId);

                var filteredPackages = packages
                    .Where(p => subscribedPackageIds.Contains(p.Id))
                    .ToList();

                var result = mapper.Map<List<PackageDto>>(filteredPackages);

                var tasks = result.Select(async dto =>
                {
                    dto.IsSubscribed = true; // always true here

                    dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);

                    var branchTasks = dto.AcssedBranches?.Select(async b =>
                        b.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(b.BranchImageURL)
                    ) ?? Enumerable.Empty<Task>();

                    var facilityTasks = dto.Facilities?.Select(async f =>
                        f.Image = await DocumentSetting.GetPreSignedUrlAsync(f.Image)
                    ) ?? Enumerable.Empty<Task>();

                    var venueTasks = dto.Venues?.Select(async v =>
                        v.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(v.VenueImageURL)
                    ) ?? Enumerable.Empty<Task>();

                    await Task.WhenAll(branchTasks.Concat(facilityTasks).Concat(venueTasks));
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception)
            {
                return new ResponseHandler()
                    .BadRequest<List<PackageDto>>("Something went wrong");
            }
        }
        public async Task<Response<PackageDto>> GetPackageById(int packageId)
        {
            try
            {
                var package = await packageRepository.GetPackageById(packageId);
                if (package != null)
                {
                    var mappedPackage = mapper.Map<PackageDto>(package);
                    // Loop through each sub-member category and fetch the member type name
                    foreach (var subMemberCategory in mappedPackage.SubMemberCategories)
                    {
                        // Call the method to get member type data
                        var response = await GetMemberTypeData(subMemberCategory.MemberTypeId, httpContextAccessor);

                        if (response.Succeeded)
                        {
                            // Set the fetched name to the sub-member category DTO
                            subMemberCategory.MemberTypeName = response.Data.Name;
                        }
                        else
                        {
                            // Handle the case where the member type data couldn't be fetched
                            subMemberCategory.MemberTypeName = "Unknown";
                        }
                    }

                    mappedPackage.Image = await DocumentSetting.GetPreSignedUrlAsync(mappedPackage.Image);

                    var branchtasks = mappedPackage.AcssedBranches.Select(async dto =>
                    {


                        dto.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(dto.BranchImageURL);
                    }).ToList() ?? Enumerable.Empty<Task>();

                    var facilityTasks = mappedPackage.Facilities?.Select(async facilityDto =>
                        {
                            facilityDto.Image = await DocumentSetting.GetPreSignedUrlAsync(facilityDto.Image);
                        }).ToList() ?? Enumerable.Empty<Task>();



                    var venueTasks = mappedPackage.Venues?.Select(async venueDto =>
                    {
                        venueDto.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(venueDto.VenueImageURL);
                    }).ToList() ?? Enumerable.Empty<Task>();

                    await Task.WhenAll(branchtasks
                            .Concat(facilityTasks)
                            .Concat(venueTasks));


                    return new ResponseHandler().Success(mappedPackage);
                }
                else
                {
                    return new ResponseHandler().NotFound<PackageDto>("Invalid packageId");
                }
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PackageDto>("Something went wrong");
            }
        }
        public async Task<Response<PackageDataDto>> GetPackageByIdAsync(int packageId)
        {
            try
            {
                var package = await packageRepository.GetPackageWithTenant(packageId);
                if (package != null)
                {
                    var mappedPackage = mapper.Map<PackageDataDto>(package);

                    if (mappedPackage.Tenant != null)
                    {
                        mappedPackage.Tenant.Image = await DocumentSetting.GetPreSignedUrlAsync(mappedPackage.Tenant.Image);
                    }
                    // Loop through each sub-member category and fetch the member type name

                    return new ResponseHandler().Success(mappedPackage);
                }
                else
                {
                    return new ResponseHandler().NotFound<PackageDataDto>("Invalid packageId");
                }
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PackageDataDto>("Something went wrong");
            }
        }

        public async Task<Response<bool>> SoftDeletePackage(int packageId)
        {
            try
            {
                await packageRepository.SoftDeletePackage(packageId);
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<List<PackageDto>>> GetPackagesByTenantIdAsync(int tenantId)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                var packages = await packageRepository.GetPackageByTenantId(tenantId);
                var result = mapper.Map<List<PackageDto>>(packages);
                // Get active package ids subscribed by user in same tenant
                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(x =>
                    x.TenantId == tenantId &&
                    x.UserId == userId &&
                    x.PackageId != null &&
                    x.MembershipStatus == MembershipStatus.Active
                );

                var memberships = await unitOfWork
                    .Repository<Data.Membership.Entities.Membership>()
                    .GetAllWithSpecifications(spec);

                var subscribedPackageIds = memberships
                    .Select(x => x.PackageId.Value)
                    .Distinct()
                    .ToList();

                var tasks = result.Select(async dto =>
                {
                    dto.IsSubscribed = subscribedPackageIds.Contains(dto.Id);

                    dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);
                    var branchTasks = dto.AcssedBranches?.Select(async branchDto =>
                    {
                        branchDto.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDto.BranchImageURL);
                    }) ?? Enumerable.Empty<Task>();
                    var facilityTasks = dto.Facilities?.Select(async facilityDto =>
                    {
                        facilityDto.Image = await DocumentSetting.GetPreSignedUrlAsync(facilityDto.Image);
                    }) ?? Enumerable.Empty<Task>();
                    var venueTasks = dto.Venues?.Select(async venueDto =>
                    {
                        venueDto.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(venueDto.VenueImageURL);
                    }) ?? Enumerable.Empty<Task>();
                    await Task.WhenAll(branchTasks
                            .Concat(facilityTasks)
                            .Concat(venueTasks));
                }).ToList();
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<PackageDto>>("Something went wrong");
            }
        }

        public async Task<Response<IReadOnlyList<GetPackageDto>>> GetAllPackagesByTenantIdAndListOfIds(int tenantId, List<int> packagesIds)
        {
            try
            {
                var packages = await packageRepository.GetPackagesByTenantIdAndByListOfIds(tenantId, packagesIds);


                if (packages.Count == 0)
                    return new ResponseHandler().NotFound<IReadOnlyList<GetPackageDto>>("Invalid tenatId or packages Ids");

                if (packages.Count < packagesIds.Count)
                    return new ResponseHandler().BadRequest<IReadOnlyList<GetPackageDto>>("Invalid packages Ids");


                var tasks = packages.Select(async dto =>
                {
                    dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);
                }).ToList();
                await Task.WhenAll(tasks);


                return new ResponseHandler().Success(packages);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<IReadOnlyList<GetPackageDto>>("Something went wrong");
            }
        }

        public async Task<Response<IReadOnlyList<GetPackageDto>>> GetAllPackagesByTenantId(int tenantId)
        {
            try
            {
                var packages = await packageRepository.GetAllPackagesByTenantId(tenantId);


                if (packages.Count == 0)
                    return new ResponseHandler().NotFound<IReadOnlyList<GetPackageDto>>("Invalid tenatId or packages Ids");

                var tasks = packages.Select(async dto =>
                {
                    dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);
                }).ToList();
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(packages);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<IReadOnlyList<GetPackageDto>>("Something went wrong");
            }
        }
        private async Task<Response<MemberTypeDto>> GetMemberTypeData(int memberTypeId, IHttpContextAccessor httpContextAccessor)
        {
            var httpClient = new HttpClient();
            //FIXME: This method gets the member type data from membership service
            //QA link: https://buildup-gateway-dev.buildup-egypt.com
            //Dev Link: https://buildup-gateway-dev.buildup-egypt.com
            // Construct the API URL with the provided member type ID

            var mainApiUrl = $"https://buildup-gateway-dev.buildup-egypt.com/EMembership/1/MemberType/GetMemberTypeById?id={memberTypeId}";

            // Extract the bearer token from the request headers
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            token = token.Replace("Bearer ", "");

            // Add the authorization header to the HTTP client
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Make the GET request to the API
            var response = await httpClient.GetAsync(mainApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Parse the JSON content into a JObject
                var jsonObject = JObject.Parse(content);

                // Access the "data" field
                var data = jsonObject["data"];

                if (data != null)
                {
                    var id = data["id"]?.Value<int>() ?? 0;
                    var name = data["name"]?.Value<string>();
                    var arabicName = data["arabicName"]?.Value<string>();

                    // Create a new MemberTypeDto with the parsed data
                    var memberTypeDto = new MemberTypeDto
                    {
                        Id = id,
                        Name = name,
                        ArabicName = arabicName
                    };

                    // Return success response with the populated DTO
                    return new ResponseHandler().Success(memberTypeDto);
                }
                else
                {
                    return new ResponseHandler().BadRequest<MemberTypeDto>("Data not found in the response");
                }
            }
            else
            {
                return new ResponseHandler().BadRequest<MemberTypeDto>("Failed to fetch member type data");
            }
        }
        private async Task<HttpResponseMessage> PostPackageRequestAuthorizationsAsync(List<PackageAuthourizationsDto> packageRequestAuthorizationDtos)
        {
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Serialize the DTO list to JSON
                var jsonContent = JsonConvert.SerializeObject(packageRequestAuthorizationDtos);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                //FIXME: This method post package request authorizations from membership service
                //QA link: https://buildup-gateway-dev.buildup-egypt.com
                //Dev Link: https://buildup-gateway-dev.buildup-egypt.com
                // Make the POST request
                var response = await httpClient.PostAsync($"{configuration["ApiSettings:Gateway"]}/EMembership/1/PackageRequestAuthorization/AddPackageRequestAuthorizations", httpContent);
                return response;
            }
        }

        public async Task<bool> IsPPackageHasAccessToVenueOrFacilityOrBranch(BranchFacilityVenueDto dto)
        {
            try
            {
                var packages = await packageRepository.GetAllPackageByTenantId(dto.TenantId);

                if (packages.Any())
                {
                    foreach (var package in packages)
                    {
                        bool hasBranchAccess = package.Branches?.Any(b => b.BranchId == dto.BranchId) ?? false;

                        bool hasFacilityAccess = package.Facilities?.Any(f => f.FacilityId == dto.FacilityId) ?? false;

                        bool hasVenueAccess = package.Venues?.Any(v => v.VenueId == dto.VenueId) ?? false;

                        // If access exists for at least one of branch, facility, or venue
                        if (hasBranchAccess || hasFacilityAccess || hasVenueAccess)
                        {
                            return true;
                        }
                    }

                    return false;
                }
                else
                {
                    return false; // No packages found for the tenant
                }
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return false;
            }
        }
        public async Task<List<PackageSummaryDto>> GetSuperPackagesAsync()
        {
            var packages = await unitOfWork.Repository<Package>()
                .GetAllAsync(p => p.SuperPackageId == null);

            var packagesDto = mapper.Map<List<PackageSummaryDto>>(packages);

            var tasks = packagesDto.Select(async dto =>
            {
                dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);
            }).ToList();
            await Task.WhenAll(tasks);

            return packagesDto;
        }

        public async Task<List<PackageSummaryDto>> GetSupPackagesAsync(int superPackageId)
        {
            var packages = await unitOfWork.Repository<Package>()
                .GetAllAsync(p => p.SuperPackageId == superPackageId);
            var packagesDto = mapper.Map<List<PackageSummaryDto>>(packages);

            var tasks = packagesDto.Select(async dto =>
            {
                dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);
            }).ToList();

            return packagesDto;
        }
        public async Task<Response<PaginationToReturnDto<BranchCardViewDto>>> GetPackageBranchesAsync(int PackageId, PackageBranchListDto branchListDetailsDto)
        {
            try
            {
                var spec = new GeneralSpecifications<AccessedBranches>(
                    ab => ab.PackageId == PackageId)
                {
                    Includes =
                    [
                        ab => ab.Include(x => x.Branch),
                    ]
                };

                var entities = await unitOfWork.Repository<AccessedBranches>()
                                               .GetAllWithSpecifications(spec);

                // Apply branch name search
                if (!string.IsNullOrWhiteSpace(branchListDetailsDto.SearchTerm))
                {
                    var search = branchListDetailsDto.SearchTerm.ToLower();
                    entities = entities.Where(ab =>
                        ab.Branch != null && ab.Branch.Name.ToLower().Contains(search)).ToList();
                }

                var totalCount = entities.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)branchListDetailsDto.PageSize);

                var pagedEntities = entities
                    .Skip((branchListDetailsDto.PageNumber - 1) * branchListDetailsDto.PageSize)
                    .Take(branchListDetailsDto.PageSize)
                    .Select(ab => new BranchCardViewDto
                    {
                        Id = ab.BranchId,
                        BranchName = ab.Branch?.Name ?? "N/A",
                        ImageURL = ab.Branch?.BranchImageURL ?? string.Empty,
                        Address = ab.Branch?.Location ?? "N/A"
                    })
                    .ToList();


                var tasks = pagedEntities.Select(async dto =>
                {
                    dto.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(dto.ImageURL);
                }).ToList();
                await Task.WhenAll(tasks);

                var result = new PaginationToReturnDto<BranchCardViewDto>(
                    totalCount, totalPages, branchListDetailsDto.PageSize, branchListDetailsDto.PageNumber, pagedEntities);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler()
                    .BadRequest<PaginationToReturnDto<BranchCardViewDto>>(ex.Message);
            }
        }
        public async Task<Response<List<BranchWithFacilitiesDto>>> GetAccessedFacilitiesWithVenuesAsync(int packageId, List<int> branchIds)
        {
            try
            {
                var spec = new GeneralSpecifications<AccessedFacilitiesVenues>(
                    afv => afv.PackageId == packageId && branchIds.Contains(afv.BranchId))
                {
                    Includes =
                    [
                        afv => afv.Include(x => x.Branch),

                        afv => afv.Include(x => x.Facility),
                afv => afv.Include(x => x.Venue)
                    ]
                };

                var entities = await unitOfWork.Repository<AccessedFacilitiesVenues>()
                                               .GetAllWithSpecifications(spec);

                if (!entities.Any())
                    return new ResponseHandler().Success(new List<BranchWithFacilitiesDto>());

                var tasks = entities.Select(async dto =>
                {
                    if (dto.Facility != null)
                    {
                        dto.Facility.FacilityImageUrl = await DocumentSetting.GetPreSignedUrlAsync(dto.Facility.FacilityImageUrl);
                    }
                    if (dto.Venue != null)
                    {
                        dto.Venue.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(dto.Venue.VenueImageURL);
                    }

                    if (dto.Branch != null)
                    {
                        dto.Branch.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(dto.Branch.BranchImageURL);
                    }

                    if (dto.Package != null)
                    {
                        dto.Package.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Package.Image);

                        if (dto.Package.Tenant != null)
                        {
                            dto.Package.Tenant.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Package.Tenant.Image);
                        }
                    }
                }).ToList();
                await Task.WhenAll(tasks);

                // Group by Branch → Facilities → Venues
                var branches = entities
                    .GroupBy(afv => new
                    {
                        afv.BranchId,
                        BranchName = afv.Branch.Name
                    })
                    .Select(branchGroup => new BranchWithFacilitiesDto
                    {
                        BranchId = branchGroup.Key.BranchId,
                        BranchName = branchGroup.Key.BranchName,
                        Facilities = branchGroup
                            .GroupBy(fg => new
                            {
                                fg.FacilityId,
                                FacilityName = fg.Facility.FacilityName,
                                FacilityImage = fg.Facility.FacilityImageUrl
                            })
                            .Select(facilityGroup => new FacilityWithVenuesDto
                            {
                                FacilityId = facilityGroup.Key.FacilityId,
                                FacilityName = facilityGroup.Key.FacilityName,
                                Image = facilityGroup.Key.FacilityImage,
                                Venues = facilityGroup
                                    .Select(v => new NewVenueDto
                                    {
                                        Id = v.VenueId,
                                        Name = v.Venue.Name,
                                        VenueImageURL = v.Venue.VenueImageURL,
                                        FacilityId = facilityGroup.Key.FacilityId

                                    })
                                    .Distinct()
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList();


                return new ResponseHandler().Success(branches);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<BranchWithFacilitiesDto>>(ex.Message);
            }
        }

        private async Task<HttpResponseMessage> GetPackageById(List<PackageAuthourizationsDto> packageRequestAuthorizationDtos)
        {
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Serialize the DTO list to JSON
                var jsonContent = JsonConvert.SerializeObject(packageRequestAuthorizationDtos);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                //FIXME: This method gets the package by it's ID from membership service
                //QA link: https://buildup-gateway-dev.buildup-egypt.com
                //Dev Link: https://buildup-gateway-dev.buildup-egypt.com
                // Make the POST request
                var response = await httpClient.PostAsync("https://buildup-gateway-dev.buildup-egypt.com/EMembership/1/PackageRequestAuthorization/AddPackageRequestAuthorizations", httpContent);
                return response;
            }
        }

    }
}
