using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.DashboardDtos;
using MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipDtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.Swimming;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.MembershipRepo;
using MyEntityMembershipService.Repositories.Membership.MembershipRequestRepository;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.SpecificationPattern;
using Newtonsoft.Json;
using Serilog;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using static MyEntityMembershipService.DTOs.Membership.DashboardDtos.MembershipGrowthRateDto;

namespace MyEntityMembershipService.Services.Membership.MembershipServ
{
    public class MembershipService : IMembershipService
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IMembershipRequestRepository requestRepository;
        private readonly IMembershipRepository membershipRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IFetchService fetchService;


        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper,
                                        IConfiguration configuration, IMembershipRequestRepository requestRepository, IMembershipRepository membershipRepository, IHttpContextAccessor httpContextAccessor, IFetchService fetchService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configuration = configuration;
            this.requestRepository = requestRepository;
            this.membershipRepository = membershipRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.fetchService = fetchService;
        }
        #region Old And Karata membership
        public async Task<Response<MembershipDto>> CreateMembership(MembershipDto membershipDto)
        {
            try
            {
                var MembershipRequest = await requestRepository.GetMembershipRequestsById(membershipDto.TenantId, membershipDto.RequestId);
                if (MembershipRequest.SuperMembershipRequestId == null)
                {
                    if (MembershipRequest.RequestType.Name == "New Membership Request" && MembershipRequest.Status == RequestStatus.PaymentComplete)
                    {
                        var membership = mapper.Map<Data.Membership.Entities.Membership>(membershipDto);
                        membership.MembershipRequests.Add(MembershipRequest);
                        membership.MembershipStatus = MembershipStatus.NotActive;

                        await unitOfWork.Repository<Data.Membership.Entities.Membership>().Add(membership);
                        await unitOfWork.Save();
                        var result = mapper.Map<MembershipDto>(membership);
                        if (MembershipRequest.SubMembers.Count == 0)
                        {
                            MembershipRequest.Status = RequestStatus.RequestResolved;
                        }
                        unitOfWork.Repository<MembershipRequest>().UpdateField(MembershipRequest, r => r.Status);
                        await unitOfWork.Save();
                        return new ResponseHandler().Success(result);
                    }
                    else
                    {
                        return new ResponseHandler().BadRequest<MembershipDto>("The Payment Status Not completed");
                    }
                }
                else
                {
                    return new ResponseHandler().BadRequest<MembershipDto>("Its Submember");

                }
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipDto>(ex.Message);
            }
        }
        public async Task<Response<MembershipDto>> CreateSubMemberMembership(subMemberMembershipDto subMemberMembershipDto)
        {
            try
            {
                var MembershipRequest = await requestRepository.GetMembershipRequestsById(subMemberMembershipDto.TenantId, subMemberMembershipDto.RequestId);
                if (MembershipRequest.RequestType.Name == "New Membership Request")
                {
                    var membership = mapper.Map<Data.Membership.Entities.Membership>(subMemberMembershipDto);
                    membership.MembershipStatus = MembershipStatus.NotActive;

                    membership.MembershipRequests.Add(MembershipRequest);
                    await unitOfWork.Repository<Data.Membership.Entities.Membership>().Add(membership);
                    await unitOfWork.Save();
                    var result = mapper.Map<MembershipDto>(membership);
                    MembershipRequest.Status = RequestStatus.RequestResolved;
                    unitOfWork.Repository<MembershipRequest>().UpdateField(MembershipRequest, r => r.Status);
                    await unitOfWork.Save();
                    return new ResponseHandler().Success(result);
                }
                else
                {
                    return new ResponseHandler().BadRequest<MembershipDto>("The Payment Status Not completed");
                }
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipDto>(ex.Message);
            }
        }
        public async Task<Response<List<MembershipDto>>> GeMembershipsByPackageId(int PackageId)
        {
            try
            {
                var memberships = await membershipRepository.GeMembershipByPackageId(PackageId);
                var membershipDtos = mapper.Map<List<MembershipDto>>(memberships);

                var tasks = membershipDtos.Select(async membershipDto =>
                {
                    membershipDto.NationalIDPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.NationalIDPhoto);
                    membershipDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.UserPhoto);
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(membershipDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MembershipDto>>(ex.Message);
            }
        }

        public async Task<Response<List<MembershipDto>>> GeMembershipsByTenantId(int tenantId)
        {
            try
            {
                var memberships = await membershipRepository.GeMembershipByTenantId(tenantId);
                var membershipDtos = mapper.Map<List<MembershipDto>>(memberships);

                var tasks = membershipDtos.Select(async membershipDto =>
                {
                    membershipDto.NationalIDPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.NationalIDPhoto);
                    membershipDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.UserPhoto);
                });

                return new ResponseHandler().Success(membershipDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MembershipDto>>(ex.Message);
            }
        }
        public async Task<Response<MembershipDto>> GeMembershipsById(int Id)
        {
            try
            {
                var membership = await membershipRepository.GetByIdAsync(Id);
                var membershipDto = mapper.Map<MembershipDto>(membership);

                return new ResponseHandler().Success(membershipDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipDto>(ex.Message);
            }
        }
        public async Task<Response<List<MembershipSegmentationDto>>> GetMembershipsSegmentationByPackageAsync(int year, int tenantId)
        {
            try
            {
                // Step 1: Get all memberships filtered by tenant ID and year
                var memberships = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetAllAsync(m => m.TenantId == tenantId
                                     && m.StartDate.HasValue
                                     && m.StartDate.Value.Year == year
                                     && (m.MembershipStatus == MembershipStatus.Active || m.MembershipStatus == MembershipStatus.Freezed));

                // Step 2: Calculate total active and frozen memberships
                var totalMemberships = memberships.Count();

                // Step 3: Group memberships by PackageId, ensuring PackageId is not null
                var packageMemberships = memberships
                    .Where(m => m.PackageId.HasValue) // Ensure PackageId is not null
                    .GroupBy(m => m.PackageId.Value)  // Use .Value to get the int
                    .Select(g => new
                    {
                        PackageId = g.Key,
                        MembershipCount = g.Count()
                    })
                    .ToList();

                // Step 4: Create segmentation DTOs
                var segmentationList = new List<MembershipSegmentationDto>();

                foreach (var package in packageMemberships)
                {
                    var packageDto = await GetPackageByIdAsync(package.PackageId);
                    var packageName = packageDto?.Name ?? "Unknown Package"; // Handle null cases

                    // Calculate the percentage
                    var percentage = totalMemberships > 0 ? package.MembershipCount / (double)totalMemberships * 100 : 0;

                    segmentationList.Add(new MembershipSegmentationDto
                    {
                        PackageName = packageName,
                        MembershipCount = package.MembershipCount,
                        PercentageShare = Math.Round(percentage, 2) // Round to 2 decimal places
                    });
                }

                // Step 5: Return the response with the segmentation data
                return new ResponseHandler().Success(segmentationList);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MembershipSegmentationDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<NumberOfMembershpsDto>> GetMembershipsGroupedByPackageNameAsync(int year, int? tenantId)
        {
            try
            {
                if (!tenantId.HasValue)
                {
                    return new ResponseHandler().BadRequest<NumberOfMembershpsDto>("tenantId Is Requried");
                }
                // Step 1: Get memberships filtered by tenant ID and year
                var memberships = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetAllAsync(m => tenantId == 1 || m.TenantId == tenantId
                                  && m.StartDate.HasValue
                                  && m.StartDate.Value.Year == year
                                  && (m.MembershipStatus == MembershipStatus.Active
                                  || m.MembershipStatus == MembershipStatus.Freezed));

                // Step 2: If there are no memberships, return 0 for all months
                if (!memberships.Any())
                {
                    // Prepopulate an empty package with 0 memberships for all months
                    var result = new Dictionary<string, List<MembershipSummaryDto>>
                    {
                        ["Unknown Package"] = Enumerable.Range(1, 12).Select(month => new MembershipSummaryDto
                        {
                            PackageId = 0, // Default ID for an unknown package
                            Month = month,
                            MonthName = new DateTime(1, month, 1).ToString("MMMM"),
                            Year = year,
                            MembershipCount = 0
                        }).ToList()
                    };

                    return new ResponseHandler().Success(new NumberOfMembershpsDto
                    {
                        PakageMembershipNumbers = result,
                        MinMembershipCount = 0,
                        Interval = 500,
                        MaxMembershipCount = 3000
                    });
                }

                // Step 3: Group memberships by PackageId and Month
                var groupedData = memberships
                    .GroupBy(m => new { m.PackageId, m.StartDate.Value.Month })
                    .Select(g => new MembershipSummaryDto
                    {
                        PackageId = g.Key.PackageId.Value,
                        Month = g.Key.Month,
                        MembershipCount = g.Count(),
                        MonthName = new DateTime(1, g.Key.Month, 1).ToString("MMMM") // Get month name
                    })
                    .ToList();

                // Step 4: Create the final structure grouped by PackageName
                var resultPackages = new Dictionary<string, List<MembershipSummaryDto>>();

                // Get all distinct PackageIds from groupedData
                var distinctPackageIds = groupedData.Select(m => m.PackageId).Distinct().ToList();

                foreach (var packageId in distinctPackageIds)
                {
                    var packageDto = await GetPackageByIdAsync(packageId);
                    var packageName = packageDto?.Name ?? "Unknown Package"; // Handle null cases

                    if (!resultPackages.ContainsKey(packageName))
                    {
                        resultPackages[packageName] = new List<MembershipSummaryDto>();
                    }

                    // Ensure each package has entries for all 12 months
                    for (int month = 1; month <= 12; month++)
                    {
                        var membershipInMonth = groupedData.FirstOrDefault(m => m.PackageId == packageId && m.Month == month);

                        if (membershipInMonth != null)
                        {
                            // Add existing membership data
                            resultPackages[packageName].Add(new MembershipSummaryDto
                            {
                                PackageId = packageDto.Id,
                                Month = month,
                                MonthName = new DateTime(1, month, 1).ToString("MMMM"),
                                Year = year,
                                MembershipCount = membershipInMonth.MembershipCount
                            });
                        }
                        else
                        {
                            // Add month entry with 0 membership count if no data exists for that month
                            resultPackages[packageName].Add(new MembershipSummaryDto
                            {
                                PackageId = packageDto.Id,
                                Month = month,
                                MonthName = new DateTime(1, month, 1).ToString("MMMM"),
                                Year = year,
                                MembershipCount = 0
                            });
                        }
                    }
                }

                var allMembershipCounts = resultPackages.SelectMany(r => r.Value.Select(m => m.MembershipCount)).ToList();

                // Use MaxBy to get the maximum membership count or 3000 if the maximum is below that threshold
                var maxMembershipCount = allMembershipCounts.Any()
                    ? allMembershipCounts.MaxBy(m => m) < 3000 ? 3000 : allMembershipCounts.MaxBy(m => m)
                    : 0;

                var NumberOfMembershps = new NumberOfMembershpsDto
                {
                    PakageMembershipNumbers = resultPackages,
                    MinMembershipCount = 0,
                    Interval = 500,
                    MaxMembershipCount = maxMembershipCount,
                };

                // Return the structured response with the packages and summary
                return new ResponseHandler().Success(NumberOfMembershps);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<NumberOfMembershpsDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task<Response<MembershipGrowthRateDto>> MembershipsGrowthRatePerPackage(int year, int? tenantId)
        {
            try
            {

                if(tenantId is null)
                {
                    return new ResponseHandler().BadRequest<MembershipGrowthRateDto>("tenantId is required");
                }
                // Step 1: Get all memberships for the tenant and year
                var memberships = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetAllAsync(m => m.TenantId == tenantId
                                     && m.StartDate.HasValue
                                     && ((DateTime)m.StartDate).Year == year
                                     && (m.MembershipStatus == MembershipStatus.Active || m.MembershipStatus == MembershipStatus.Freezed));

                // Step 2: Pre-fetch all cancelled memberships for the year in ONE query
                var allCancelledMemberships = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetAllAsync(m => m.TenantId == tenantId
                                     && m.StartDate.HasValue
                                     && ((DateTime)m.StartDate).Year == year
                                     && m.MembershipStatus == MembershipStatus.Cancelled);

                // Step 3: Group memberships by PackageId and Month
                var groupedData = memberships
                    .Where(m => m.PackageId.HasValue)
                    .GroupBy(m => new { m.PackageId, m.StartDate!.Value.Month })
                    .Select(g => new MembershipSummaryDto
                    {
                        PackageId = g.Key.PackageId!.Value,
                        Month = g.Key.Month,
                        MembershipCount = g.Count(),
                        MonthName = new DateTime(1, g.Key.Month, 1).ToString("MMMM")
                    })
                    .ToList();

                // Step 4: Get distinct package IDs
                var distinctPackageIds = groupedData.Select(m => m.PackageId).Distinct().ToList();

                var result = new Dictionary<string, List<MembershipGrowthRateSummaryDto>>();

                if (!distinctPackageIds.Any())
                {
                    return new ResponseHandler().Success(new MembershipGrowthRateDto
                    {
                        PackageGrowthRates = result,
                        MinMembershipCount = 0,
                        Interval = 10,
                        MaxMembershipCount = 100,
                    });
                }

                // Step 5: Process each package and calculate monthly growth rates
                foreach (var packageId in distinctPackageIds)
                {
                    var packageDto = await GetPackageByIdAsync(packageId);
                    var packageName = packageDto?.Name ?? "Unknown Package";

                    if (!result.ContainsKey(packageName))
                    {
                        result[packageName] = new List<MembershipGrowthRateSummaryDto>();
                    }

                    for (int month = 1; month <= 12; month++)
                    {
                        var membershipInMonth = groupedData
                            .FirstOrDefault(m => m.PackageId == packageId && m.Month == month);

                        var currentTotal = membershipInMonth?.MembershipCount ?? 0;

                        double growthRate = 0;

                        if (month > 1)
                        {
                            var previousMemberships = groupedData
                                .FirstOrDefault(m => m.PackageId == packageId && m.Month == month - 1);

                            int previousTotal = previousMemberships?.MembershipCount ?? 0;

                            // FIX: filter cancelled memberships from the pre-fetched list in memory
                            // instead of issuing a new DB query per month per package.
                            int cancelledCount = allCancelledMemberships
                                .Count(m => m.PackageId == packageId
                                            && ((DateTime)m.StartDate!).Month == month);

                            if (previousTotal == 0 && currentTotal > 0)
                            {
                                growthRate = 100;
                            }
                            else if (previousTotal > 0)
                            {
                                growthRate = (currentTotal - cancelledCount) / (double)previousTotal * 100;
                            }
                        }

                        result[packageName].Add(new MembershipGrowthRateSummaryDto
                        {
                            PackageId = packageDto!.Id,
                            Month = month,
                            MonthName = new DateTime(1, month, 1).ToString("MMMM"),
                            Year = year,
                            GrowthRate = growthRate
                        });
                    }
                }

                var response = new MembershipGrowthRateDto
                {
                    PackageGrowthRates = result,
                    MinMembershipCount = 0,
                    Interval = 10,
                    MaxMembershipCount = 100,
                };

                return new ResponseHandler().Success(response);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipGrowthRateDto>(
                    ex.InnerException?.Message ?? ex.Message);
            }
        }

        // Method to call the external API and get package details
        public async Task<PackageDto> GetPackageByIdAsync(int packageId)
        {
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var url = $"{configuration["ApiSettings:Gateway"]}/EMembership/1/Package/GetPackageById/{packageId}";

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Response<PackageDto>>(jsonResponse);
                    return result?.Data;
                }
                else
                {
                    throw new Exception($"Failed to retrieve package data. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }
            }
        }

        public async Task<Response<List<TenantMemberDto>>> GetTenantMembers(GetTenantMembersParamDto parameters)
        {
            try
            {
                var totalCount = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetCountByCriteriaAsync(m => m.TenantId == parameters.TenantId &&
                                (!parameters.Status.HasValue || m.MembershipStatus == (MembershipStatus)parameters.Status) &&
                                (string.IsNullOrEmpty(parameters.SearchName) || m.FirstName.ToLower().Contains(parameters.SearchName.ToLower()) || m.LastName.ToLower().Contains(parameters.SearchName.ToLower()) || m.FileNumber.Contains(parameters.SearchName)) &&
                                (string.IsNullOrEmpty(parameters.BeltName) || m.BeltName == parameters.BeltName));

                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(m => m.TenantId == parameters.TenantId &&
                                (!parameters.Status.HasValue || m.MembershipStatus == (MembershipStatus)parameters.Status) &&
                                (string.IsNullOrEmpty(parameters.SearchName) || m.FirstName.ToLower().Contains(parameters.SearchName.ToLower()) || m.LastName.ToLower().Contains(parameters.SearchName.ToLower()) || m.FileNumber.Contains(parameters.SearchName)) &&
                                (string.IsNullOrEmpty(parameters.BeltName) || m.BeltName == parameters.BeltName),
                                parameters.PageIndex, parameters.PageSize);
                var memberships = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetAllWithSpecifications(spec);
                var result = mapper.Map<List<TenantMemberDto>>(memberships);
                return new ResponseHandler().Success(result, new { NumberOfPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize) });

                #region CodeBeforeRefactor
                //GeneralSpecifications<Data.Membership.Entities.Membership> spec;
                //if (parameters.Status.HasValue)
                //    spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(m => m.MembershipStatus == (MembershipStatus)parameters.Status && m.TenantId == parameters.TenantId);
                //else
                //    spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(m => m.TenantId == parameters.TenantId);
                //// To Get Form User Response To be able to filter with dynamic field (BeltId) that doesn't exist in Membership table
                //spec.Includes = new List<Func<IQueryable<Data.Membership.Entities.Membership>, IQueryable<Data.Membership.Entities.Membership>>>
                //{
                //    m => m.Include(m => m.MembershipRequests).ThenInclude(request => request.FormUserResponse).ThenInclude(response => response.FieldsResponse)
                //};
                //var memberships = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetAllWithSpecifications(spec);
                //var resultMemberships = mapper.Map<List<TenantMemberDto>>(memberships);

                //foreach (var member in memberships)
                //{
                //    var newMembershipRequest = member.MembershipRequests.FirstOrDefault(request => request.RequestTypeId == 6);
                //    if (newMembershipRequest is not null)
                //    {
                //        var response = await unitOfWork.Repository<FormUserResponse>().FindAllByCriteriaAsync(res => res.MembershipRequestId == newMembershipRequest.Id);
                //        var beltResponseField = newMembershipRequest.FormUserResponse?.FieldsResponse.FirstOrDefault(r => r.FieldId == 83);
                //        resultMemberships.Find(item => item.Id == member.Id).BeltName = beltResponseField?.Response;
                //    }

                //}
                //if (!string.IsNullOrEmpty(parameters.BeltName))
                //    resultMemberships = resultMemberships.Where(item => item.BeltName == parameters.BeltName).ToList();
                //var totalCount = resultMemberships.Count;
                //resultMemberships = resultMemberships.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();

                //return new ResponseHandler().Success(resultMemberships, new { NumberOfPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize) });

                #endregion

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<TenantMemberDto>>("Something went wrong");
            }
        }

        public async Task<Response<List<FederationMemberDto>>> GetFederationMembers(GetFederationMembersParamDto parameters)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();
                var tenantAdmins = await fetchService.GetTenantAdmin(userId);
                var areasId = tenantAdmins.TenantAreaAdmins.Select(area => area.AreaId).ToList();
                int.TryParse(httpContextAccessor.HttpContext.User.GetTenantId(), out int tenantId);
                if (tenantId == 0)
                    return new ResponseHandler().BadRequest<List<FederationMemberDto>>("invalid federation Id");


                var totalCount = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetCountByCriteriaAsync(m => m.Tenant.SuperEntityId == tenantId &&
                                (!parameters.Status.HasValue || m.MembershipStatus == (MembershipStatus)parameters.Status) &&
                                (string.IsNullOrEmpty(parameters.SearchName) || m.FileNumber.ToLower().Contains(parameters.SearchName.ToLower()) || m.FirstName.ToLower().Contains(parameters.SearchName.ToLower()) || m.LastName.ToLower().Contains(parameters.SearchName.ToLower())) &&
                                (string.IsNullOrEmpty(parameters.BeltName) || m.BeltName == parameters.BeltName) &&
                                (!parameters.BranchId.HasValue || m.Tenant.AreaId == parameters.BranchId.Value) &&
                                (!areasId.Any() || areasId.Contains(m.Tenant.AreaId ?? 0)));

                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(m => m.Tenant.SuperEntityId == tenantId &&
                                (!parameters.Status.HasValue || m.MembershipStatus == (MembershipStatus)parameters.Status) &&
                                (string.IsNullOrEmpty(parameters.SearchName) || m.FileNumber.ToLower().Contains(parameters.SearchName.ToLower()) || m.FirstName.ToLower().Contains(parameters.SearchName.ToLower()) || m.LastName.ToLower().Contains(parameters.SearchName.ToLower())) &&
                                (!parameters.BranchId.HasValue || m.Tenant.AreaId == parameters.BranchId.Value) &&
                                (string.IsNullOrEmpty(parameters.BeltName) || m.BeltName == parameters.BeltName) && (!areasId.Any() || areasId.Contains(m.Tenant.AreaId ?? 0)),
                                parameters.PageIndex, parameters.PageSize);
                spec.AddInclude(x => x.Include(m => m.Tenant).ThenInclude(t => t.Area));
                var memberships = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetAllWithSpecifications(spec);
                var result = mapper.Map<List<FederationMemberDto>>(memberships);
                return new ResponseHandler().Success(result, new { NumberOfPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize) });


            }
            catch (Exception ex)
            {
                Log.Error(ex, "error while getting Federtaion Members");
                return new ResponseHandler().BadRequest<List<FederationMemberDto>>("Something went wrong");
            }
        }

        public async Task<Response<List<DropdownDto>>> GetBelts()
        {
            try
            {
                var belts = await unitOfWork.Repository<DropdownValue>().FindAllByCriteriaAsync(value => value.FieldId == 83);
                var result = mapper.Map<List<DropdownDto>>(belts);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<DropdownDto>>("Something went wrong");
            }
        }

        public async Task<Response<PaginationToReturnDto<MembershipInjuryStatusDto>>> GetSwimmingMembers(GetSwimmingMembersParamDto parameters)
        {
            try
            {
                var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

                if(parameters.UserId == null || string.IsNullOrEmpty(parameters.UserId))
                {
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipInjuryStatusDto>>("UserId is required");
                }

                //var isPrivileged = parameters.IsSuperAdmin || parameters.IsTenantAdmin;
                var isPrivileged = false;

                List<int> userBranchIds = [];

                if (!isPrivileged)
                {
                    var tenantAdminSpec = new GeneralSpecifications<TenantAdmin>(
                        ta => ta.UserId == parameters.UserId
                           && ta.TenantId == tenantId
                    );
                    tenantAdminSpec.AddInclude(x => x.Include(x => x.TenantAdminBranches));

                    var tenantAdmin = await unitOfWork.Repository<TenantAdmin>()
                                                      .GetWithSpecifications(tenantAdminSpec);

                    if (tenantAdmin == null)
                        return new ResponseHandler().NotFound<PaginationToReturnDto<MembershipInjuryStatusDto>>("Tenant admin not found");

                    userBranchIds = tenantAdmin.TenantAdminBranches
                        .Select(b => b.BranchId)
                        .Distinct()
                        .ToList();
                }


                // ensure sane paging values
                var pageIndex = Math.Max(1, parameters.PageIndex);
                var pageSize = Math.Max(1, parameters.PageSize);
                var normalizedSearch = string.IsNullOrWhiteSpace(parameters.SearchName)
                    ? null
                    : parameters.SearchName.Replace(" ", "").ToLower();



                Expression<Func<Data.Membership.Entities.Membership, bool>> criteria = m =>
                    m.TenantId == tenantId
                    && m.MembershipStatus == MembershipStatus.Active

                    // Branch scope: bypass entirely for SuperAdmin/TenantAdmin
                    && (isPrivileged || m.MembershipAccesses.Any(a => userBranchIds.Contains(a.BranchId)))

                    && (!parameters.Gender.HasValue || m.Gender.Value == parameters.Gender.Value)
                    && (!parameters.CategoryId.HasValue || m.MemberSportLevels.Any(l => l.CategoryId == parameters.CategoryId.Value))

                    && (!parameters.HasInjury.HasValue
                        || m.InjuryStatus == (InjuryStatus)parameters.HasInjury.Value)

                    && (!parameters.BirthDateFrom.HasValue
                        || (m.BirthDate.HasValue && m.BirthDate.Value >= parameters.BirthDateFrom.Value))
                    && (!parameters.BirthDateTo.HasValue
                        || (m.BirthDate.HasValue && m.BirthDate.Value <= parameters.BirthDateTo.Value))

                    && (
                        parameters.BranchIds == null
                        || !parameters.BranchIds.Any()
                        // Privileged: filter by requested branches only
                        // Non-privileged: intersect requested branches with accessible branches
                        || m.MembershipAccesses.Any(a =>
                            parameters.BranchIds.Contains(a.BranchId)
                            && (isPrivileged || userBranchIds.Contains(a.BranchId)))
                    )

                    && (!parameters.SportId.HasValue || m.MemberSportLevels.Any(l => l.SportId == parameters.SportId.Value))
                    && (!parameters.LevelId.HasValue || m.MemberSportLevels.Any(l => l.OptionId == parameters.LevelId.Value))

                    && (
                        string.IsNullOrEmpty(normalizedSearch)
                        ||
                        EF.Functions.Like(
                            ((m.FirstName ?? "") +
                             (m.SecondName ?? "") +
                             (m.MiddleName ?? "") +
                             (m.LastName ?? "")).Replace(" ", "").ToLower(),
                            $"%{normalizedSearch}%"
                        )
                        || (m.NationalId != null && m.NationalId.Contains(parameters.SearchName))
                        || (m.PhoneNumber != null && m.PhoneNumber.Contains(parameters.SearchName))
                        || (m.FileNumber != null && m.FileNumber.Contains(parameters.SearchName))
                    );


                // 1) Fetch all matching membership rows (no paging) — we will dedupe to latest per user
                var specAll = new GeneralSpecifications<Data.Membership.Entities.Membership>(criteria);
                specAll.AddInclude(x => x.Include(x => x.MembershipAccesses).ThenInclude(pp => pp.Branch));
                specAll.AddInclude(x => x.Include(x => x.MemberSportLevels));

                specAll.SetOrderByDescending(m => m.CreationDate); // optional, helps ordering in memory
                var allMatching = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                                                  .GetAllWithSpecifications(specAll);

                if (allMatching == null || !allMatching.Any())
                    return new ResponseHandler().NotFound<PaginationToReturnDto<MembershipInjuryStatusDto>>("No memberships found");

                // 2) Group by UserId and take the latest membership per user
                //    Use (m.UserId ?? string.Empty) to avoid null-group surprises
                var latestPerUser = allMatching
                    .GroupBy(m => m.UserId ?? string.Empty)
                    .Select(g => g.OrderByDescending(m => m.CreationDate).First())
                    .OrderByDescending(m => m.CreationDate) // order users by their latest membership date
                    .ToList();

                // 3) Pagination on distinct users
                var totalDistinctUsers = latestPerUser.Count;
                var pagedLatest = latestPerUser
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 4) Map to DTO
                var result = pagedLatest.Select(m => new MembershipInjuryStatusDto
                {
                    Id = m.Id,
                    UserId = m.UserId ?? string.Empty,
                    FirstName = m.FirstName ?? string.Empty,
                    LastName = m.LastName ?? string.Empty,
                    FullName = $"{m.FirstName} {m.SecondName} {m.MiddleName} {m.LastName}",
                    PhoneNumber = m.PhoneNumber ?? string.Empty,
                    NationalId = m.NationalId ?? string.Empty,
                    DateOfAcquisition = m.CreationDate ?? DateTime.MinValue,
                    Gender = m.Gender ?? Gender.Male,
                    Levels = m.MemberSportLevels
                    .OrderByDescending(l => l.CreationDate) 
                    .Take(1)
                    .Select(l => new MemberLevelDto
                    {
                        LevelId = l.LevelId,
                        LevelName = l.OptionName,
                        SportId = l.SportId,
                        SportName = l.SportName,
                        LevelAndSport = $"{l.SportName}-{l.OptionName}",
                        LevelAndSportAr = $"{l.SportNameAr}-{l.OptionNameAr}",
                        CategoryId = l.CategoryId,
                        CategoryName = l.CategoryName,
                        CategoryNameAr = l.CategoryNameAr
                    }).ToList(),
                    InjuryStatus = m.InjuryStatus ?? InjuryStatus.Uninjured,
                    ImageUrl = m.UserPhoto ?? string.Empty,
                    Email = m.Email ?? string.Empty,
                    BirthDate = m.BirthDate ?? DateTime.MinValue,
                    BranchNames = m.MembershipAccesses
                    .Where(a => a.Branch != null)
                    .Select(a => a.Branch.Name)
                    .Distinct()
                    .ToList(),
                }).ToList().AsReadOnly();

                var tasks = result.Select(async membershipDto =>
                {
                    membershipDto.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.ImageUrl);
                }).ToList();

                await Task.WhenAll(tasks);

                var pagedResult = new PaginationToReturnDto<MembershipInjuryStatusDto>(
                    count: totalDistinctUsers,
                    totalPages: (int)Math.Ceiling((double)totalDistinctUsers / pageSize),
                    pageSize: pageSize,
                    pageNumber: pageIndex,
                    data: result
                );

                return new ResponseHandler().Success(pagedResult);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<MembershipInjuryStatusDto>>("Something went wrong");
            }
        }

        public async Task<Response<List<MembershipInjuryStatusDto>>> GetAllSwimmingMembers()
        {
            try
            {
                var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

                Expression<Func<Data.Membership.Entities.Membership, bool>> criteria = m =>
                    m.TenantId == tenantId
                    && m.MembershipStatus == MembershipStatus.Active;

                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(criteria);
                spec.AddInclude(x => x.Include(x => x.MembershipAccesses).ThenInclude(pp => pp.Branch));
                spec.AddInclude(x => x.Include(x => x.MemberSportLevels));

                spec.SetOrderByDescending(m => m.CreationDate);

                var allMatching = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                                                  .GetAllWithSpecifications(spec);

                if (allMatching == null || !allMatching.Any())
                    return new ResponseHandler().Success<List<MembershipInjuryStatusDto>>(new List<MembershipInjuryStatusDto>());

                // Dedupe to latest membership per user
                var latestPerUser = allMatching
                    .GroupBy(m => m.UserId ?? string.Empty)
                    .Select(g => g.OrderByDescending(m => m.CreationDate).First())
                    .OrderByDescending(m => m.CreationDate)
                    .ToList();

                var result = latestPerUser.Select(m => new MembershipInjuryStatusDto
                {
                    Id = m.Id,
                    UserId = m.UserId ?? string.Empty,
                    FirstName = m.FirstName ?? string.Empty,
                    LastName = m.LastName ?? string.Empty,
                    FullName = $"{m.FirstName} {m.SecondName} {m.MiddleName} {m.LastName}",
                    PhoneNumber = m.PhoneNumber ?? string.Empty,
                    NationalId = m.NationalId ?? string.Empty,
                    DateOfAcquisition = m.CreationDate ?? DateTime.MinValue,
                    Gender = m.Gender ?? Gender.Male,
                    Levels = m.MemberSportLevels
                    .Select(l => new MemberLevelDto
                    {
                        LevelId = l.LevelId,
                        LevelName = l.OptionName,
                        SportId = l.SportId,
                        SportName = l.SportName
                    })
                    .ToList(),
                    InjuryStatus = m.InjuryStatus ?? InjuryStatus.Uninjured,
                    ImageUrl = m.UserPhoto ?? string.Empty,
                    Email = m.Email ?? string.Empty,
                    BirthDate = m.BirthDate ?? DateTime.MinValue,
                    BranchNames = m.MembershipAccesses
                        .Where(a => a.Branch != null)
                        .Select(a => a.Branch.Name)
                        .Distinct()
                        .ToList(),
                }).ToList();

                var tasks = result.Select(async membershipDto =>
                {
                    membershipDto.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.ImageUrl);
                }).ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success<List<MembershipInjuryStatusDto>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MembershipInjuryStatusDto>>("Something went wrong");
            }
        }

        public async Task<Response<MembershipInjuryStatusDto>> GetMembershipInjuryStatusAsync(int membershipId)
        {
            try
            {

                Expression<Func<Data.Membership.Entities.Membership, bool>> criteria = m => m.Id == membershipId;

                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(criteria);
                spec.AddInclude(x => x.Include(m => m.Package));
                spec.AddInclude(x => x.Include(x => x.MemberSportLevels));


                var entity = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                                              .GetWithSpecifications(spec);

                if (entity == null)
                {
                    return new ResponseHandler().NotFound<MembershipInjuryStatusDto>("Membership not found");
                }

                var data = new MembershipInjuryStatusDto
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    FullName = $"{entity.FirstName} {entity.SecondName} {entity.MiddleName} {entity.LastName}",
                    PhoneNumber = entity.PhoneNumber,
                    NationalId = entity.NationalId,
                    BirthDate = entity.BirthDate ?? DateTime.MinValue, // Default value if null
                    DateOfAcquisition = DateTime.TryParse(entity.DateOfAcquisition, out var date) ? date : (DateTime?)null,
                    Gender = entity.Gender ?? Gender.Male,
                    Email = entity.Email ?? "-",
                    Levels = entity.MemberSportLevels
                    .Select(l => new MemberLevelDto
                    {
                        LevelId = l.LevelId,
                        LevelName = l.OptionName,
                        SportId = l.SportId,
                        SportName = l.SportName
                    })
                    .ToList(),
                    InjuryStatus = entity.InjuryStatus ?? InjuryStatus.Uninjured,
                    ImageUrl = entity.UserPhoto,
                    Issued = entity.StartDate ?? DateTime.MinValue,
                    Expiry = entity.EndDate ?? DateTime.MinValue,
                    MembershipNumber = entity.MemberShipId,
                    Package = (mapper.Map<DTOs.Entity.PackageDtos.PackageSummaryDto>(entity.Package)) ?? null

                };

                data.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(data.ImageUrl);

                if (data.Package != null)
                {
                    data.Package.Image = await DocumentSetting.GetPreSignedUrlAsync(data.Package.Image);
                }



                return new ResponseHandler().Success(data);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MembershipInjuryStatusDto>("Something went wrong");
            }
        }
        public async Task<Response<List<MembershipBasicDto>>> SearchMembershipsAsync(string? searchTerm)
        {
            var tenantIdString = httpContextAccessor.HttpContext?.User?.GetTenantId();

            if (!int.TryParse(tenantIdString, out var tenantId))
                return new ResponseHandler().BadRequest<List<MembershipBasicDto>>("Something went wrong");

            if (string.IsNullOrWhiteSpace(searchTerm))
                return new ResponseHandler().Success(new List<MembershipBasicDto>());

            searchTerm = searchTerm.Trim().ToLower();

            Expression<Func<Data.Membership.Entities.Membership, bool>> criteria = m =>
                m.TenantId == tenantId &&
                (
                    (m.FirstName + " " + m.LastName)
                        .ToLower()
                        .StartsWith(searchTerm) ||
                    m.FirstName.ToLower().StartsWith(searchTerm) ||
                    m.LastName.ToLower().StartsWith(searchTerm) ||
                    m.PhoneNumber.ToLower().Contains(searchTerm) ||
                    m.NationalId.ToLower().Contains(searchTerm) ||
                    m.Email.ToLower().Contains(searchTerm)
                );

            var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(criteria);
            var result = await unitOfWork.Repository<Data.Membership.Entities.Membership>().GetAllWithSpecifications(spec);

            var data = result.Select(m => new MembershipBasicDto
            {
                Id = m.Id,
                FullName = $"{m.FirstName} {m.SecondName} {m.MiddleName} {m.LastName}".Trim(),
                ImageUrl = m.UserPhoto,
                UserId = m.UserId
            }).ToList();

            var tasks = data.Select(async membershipDto =>
            {
                membershipDto.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.ImageUrl);
            }).ToList();


            return new ResponseHandler().Success(data);
        }
        public async Task<Response<PaginationToReturnDto<MemberSportLevelDto>>> GetMembersSportLevels(GetMemberSportLevelsParamDto parameters)
        {
            try
            {
                var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());
                //var tenantId = 2;
                var pageIndex = Math.Max(1, parameters.PageIndex);
                var pageSize = Math.Max(1, parameters.PageSize);

                var normalizedSearch = string.IsNullOrWhiteSpace(parameters.SearchName)
                    ? null
                    : parameters.SearchName.Replace(" ", "").ToLower();

                Expression<Func<MemberSportLevel, bool>> criteria = m =>

                    m.TenantId == tenantId
                    && m.Membership.MembershipStatus == MembershipStatus.Active

                    && (!parameters.Gender.HasValue || m.Membership.Gender == parameters.Gender)

                    && (parameters.SportId == null || m.SportId == parameters.SportId)

                    && (parameters.LevelId == null || m.LevelId == parameters.LevelId)

                    && (!parameters.BirthDateFrom.HasValue ||
                        (m.Membership.BirthDate.HasValue &&
                         m.Membership.BirthDate.Value >= parameters.BirthDateFrom.Value))

                    && (!parameters.BirthDateTo.HasValue ||
                        (m.Membership.BirthDate.HasValue &&
                         m.Membership.BirthDate.Value <= parameters.BirthDateTo.Value))

                    && (
                        parameters.BranchIds == null
                        || parameters.BranchIds.Count == 0
                        || m.Membership.MembershipAccesses
                            .Any(a => parameters.BranchIds.Contains(a.BranchId))
                    )

                    && (
                        string.IsNullOrEmpty(normalizedSearch)

                        || EF.Functions.Like(
                            (
                                (m.Membership.FirstName ?? "") +
                                (m.Membership.SecondName ?? "") +
                                (m.Membership.MiddleName ?? "") +
                                (m.Membership.LastName ?? "")
                            ).Replace(" ", "").ToLower(),
                            $"%{normalizedSearch}%"
                        )

                        || (m.Membership.NationalId != null &&
                            m.Membership.NationalId.Contains(parameters.SearchName))

                        || (m.Membership.PhoneNumber != null &&
                            m.Membership.PhoneNumber.Contains(parameters.SearchName))
                    );

                var spec = new GeneralSpecifications<MemberSportLevel>(criteria);

                spec.AddInclude(x => x.Include(x => x.Membership)
                                      .ThenInclude(m => m.MembershipAccesses)
                                      .ThenInclude(a => a.Branch));

                spec.SetOrderByDescending(x => x.CreationDate);

                var allData = await unitOfWork.Repository<MemberSportLevel>()
                    .GetAllWithSpecifications(spec);

                if (allData == null || !allData.Any())
                {
                    var emptyResult = new PaginationToReturnDto<MemberSportLevelDto>(
                        count: 0,
                        totalPages: 0,
                        pageSize: pageSize,
                        pageNumber: pageIndex,
                        data: new List<MemberSportLevelDto>().AsReadOnly()
                    );

                    return new ResponseHandler().Success(emptyResult);
                }

                var totalCount = allData.Count;

                var pagedData = allData
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = pagedData.Select(x =>
                {
                    var m = x.Membership;

                    return new MemberSportLevelDto
                    {
                        MembershipId = x.MembershipId,
                        UserId = x.UserId,
                        
                        Name = $"{m.FirstName} {m.SecondName} {m.MiddleName} {m.LastName}",

                        DateOfBirth = m.BirthDate ?? DateTime.MinValue,
                        Gender = m.Gender ?? Gender.Male,

                        Sport = x.SportName,
                        SportNameAr=x.SportNameAr,
                        SportLevel = x.OptionName,

                        BranchNames = m.MembershipAccesses
                            .Where(a => a.Branch != null)
                            .Select(a => a.Branch.Name)
                            .Distinct()
                            .DefaultIfEmpty("N/A") 
                            .ToList()
                    };
                }).ToList().AsReadOnly();

                var pagedResult = new PaginationToReturnDto<MemberSportLevelDto>(
                    count: totalCount,
                    totalPages: (int)Math.Ceiling((double)totalCount / pageSize),
                    pageSize: pageSize,
                    pageNumber: pageIndex,
                    data: result
                );

                return new ResponseHandler().Success(pagedResult);
            }
            catch
            {
                return new ResponseHandler()
                    .BadRequest<PaginationToReturnDto<MemberSportLevelDto>>("Something went wrong");
            }
        }        
        
        #endregion
        #region Swimming Membership
        public async Task<Response<MembershipDto>> CreateSwimmingMembership(CreateMembershipDto createMembershipDto)
        {
            try
            {
                Log.Information("Starting Add membe to the member with userId", createMembershipDto.UserId);
                var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

                var membership = mapper.Map<Data.Membership.Entities.Membership>(createMembershipDto);
                membership.UserPhoto = createMembershipDto.UserPhoto != null ? await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", createMembershipDto.UserPhoto) : null;

                membership.MembershipStatus = MembershipStatus.Active;
                membership.TenantId = tenantId;
                membership.InjuryStatus = InjuryStatus.Uninjured;
                membership.MemberShipId = GenerateMembershipId();

                await unitOfWork.Repository<Data.Membership.Entities.Membership>().Add(membership);
                await unitOfWork.Save();

                var result = mapper.Map<MembershipDto>(membership);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding membership form Dto: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<MembershipDto>(ex.Message);
            }
        }
        private string GenerateMembershipId()
        {

            Guid guid = Guid.NewGuid();


            int uniqueNumber = Math.Abs(BitConverter.ToInt32(guid.ToByteArray(), 0));


            string timestamp = DateTime.Now.Ticks.ToString();


            var uniqueId = uniqueNumber.ToString() + timestamp;

            return uniqueId;
        }
        public async Task<Response<MembershipDto>> UpdateSwimmingMembership(UpdateMembershipDto updateDto)
        {
            try
            {

                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetByIdAsync(updateDto.Id);

                if (membership == null)
                    return new ResponseHandler().NotFound<MembershipDto>("Membership not found");

                // Map new values
                mapper.Map(updateDto, membership);

                // Update photo if provided
                if (updateDto.UserPhoto != null)
                {
                    membership.UserPhoto = await DocumentSetting.AddToContainerAndGetUrl(
                        Environment.GetEnvironmentVariable("StorageConnectionString"),
                        "buildupimagedocs",
                        updateDto.UserPhoto
                    );
                }

                unitOfWork.Repository<Data.Membership.Entities.Membership>().Update(membership);
                await unitOfWork.Save();

                var result = mapper.Map<MembershipDto>(membership);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating membership: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<MembershipDto>(ex.Message);
            }
        }
        public async Task<Response<string>> SoftDeleteSwimmingMembership(int membershipId)
        {
            try
            {

                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetByIdAsync(membershipId);

                if (membership == null)
                    return new ResponseHandler().NotFound<string>("Membership not found");


                unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .SoftDelete(membership);

                await unitOfWork.Save();

                return new ResponseHandler().Success("Membership soft deleted successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error soft deleting membership: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }
        }
        public async Task<Response<string>> UnSoftDeleteSwimmingMembership(int membershipId)
        {
            try
            {

                var membership = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetByIdAsync(membershipId);

                if (membership == null)
                    return new ResponseHandler().NotFound<string>("Membership not found");


                unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .UnSoftDelete(membership);

                await unitOfWork.Save();

                return new ResponseHandler().Success("Membership soft deleted successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error soft deleting membership: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<string>(ex.Message);
            }
        }

        public async Task<Response<List<GetAllUserMemberships>>> GetAllUserMembershipsAsync(List<string> ChildrenIds)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();
                var allUserIds = new List<string> { userId };
                if (ChildrenIds != null && ChildrenIds.Any())
                    allUserIds.AddRange(ChildrenIds);

                // Criteria: memberships for user or children
                Expression<Func<Data.Membership.Entities.Membership, bool>> criteria = m => allUserIds.Contains(m.UserId);

                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(criteria);
                spec.AddInclude(entity => entity.Include(entity => entity.Tenant));
                spec.AddInclude(entity => entity.Include(entity => entity.Package).ThenInclude(pp => pp.Branches).ThenInclude(pp => pp.Branch));

                var entities = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                                               .GetAllWithSpecifications(spec);

                if (entities == null || !entities.Any())
                    return new ResponseHandler().Success(new List<GetAllUserMemberships>());

                var dto = mapper.Map<List<GetAllUserMemberships>>(entities);

                var tasks = dto.Select(async membershipDto =>
                {
                    membershipDto.TenantImage = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.TenantImage);

                    membershipDto.UserPhoto = await DocumentSetting.GetPreSignedUrlAsync(membershipDto.UserPhoto);


                }).ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<GetAllUserMemberships>>("Something went wrong");
            }
        }

        public async Task<Response<SportDistributionDto>> GetSportDistribution(SportDistributionFilterDto filterDto, int? tenantId)
        {
            try
            {
                if (!tenantId.HasValue)
                {
                    Log.Warning("Tenant ID is missing for GetSportDistribution");
                    return new ResponseHandler().BadRequest<SportDistributionDto>("Tenant ID is required");
                }

                var query = unitOfWork.Repository<MemberSportLevel>().AsQueryableAsync()
                    .Where(x => x.TenantId == tenantId.Value);

                if (filterDto.Year.HasValue)
                    query = query.Where(x => x.CreationDate.HasValue && x.CreationDate.Value.Year == filterDto.Year.Value);

                var grouped = await query
                    .GroupBy(x => new { x.SportId, x.SportName,x.SportNameAr })
                    .Select(g => new SportAthleteCountDto
                    {
                        SportId = g.Key.SportId,
                        SportName = g.Key.SportName,
                        SportNameAr = g.Key.SportNameAr,
                        AthleteCount = g.Select(x => x.UserId).Distinct().Count()
                    })
                    .OrderByDescending(x => x.AthleteCount)
                    .ToListAsync();

                var result = new SportDistributionDto
                {
                    TotalAthletes = grouped.Sum(x => x.AthleteCount),
                    Sports = grouped
                };

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting sport distribution: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<SportDistributionDto>("Something went wrong");
            }
        }


        #endregion


        public async Task<Response<bool>> UserHasMainMembershipAsync(int tenantId)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                Expression<Func<Data.Membership.Entities.Membership, bool>> criteria =
                    m => m.UserId == userId
                         && m.TenantId == tenantId
                         && m.MemberTypeId == 5 ;

                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(criteria);

                var memberships = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                                                   .GetAllWithSpecifications(spec);

                var hasMainMembership = memberships != null && memberships.Any();

                return new ResponseHandler().Success(hasMainMembership);
            }
            catch (Exception)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<List<EntitiesResponseDto>>> GetEntitiesByUserIdAsync(string userId)
        {
            try
            {
                if(string.IsNullOrEmpty(userId))
                {
                    return new ResponseHandler().BadRequest<List<EntitiesResponseDto>>("User ID is required");
                }


                Expression<Func<Data.Membership.Entities.Membership, bool>> criteria = m => m.UserId == userId;

                Expression<Func<Data.Membership.Entities.Membership, EntitiesResponseDto>> selector = m => new EntitiesResponseDto
                {
                    TenantId = m.Tenant.Id,
                    TenantName = m.Tenant.Name,
                    TenantImage = m.Tenant.Image,
                };

                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(criteria);
                var entities = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                                               .GetAllWithSpecificationsAndProjection(spec, selector);


                Log.Information("Retrieved {Count} entities for user ID {UserId}", entities.Count, userId);

                var tasks = entities.Select(async entity =>
                {
                    entity.TenantImage = await DocumentSetting.GetPreSignedUrlAsync(entity.TenantImage);
                }).ToList();

                await Task.WhenAll(tasks);

                Log.Information("Successfully processed tenant images for user ID {UserId}", userId);

                return new ResponseHandler().Success(entities);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting entities by user ID: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<List<EntitiesResponseDto>>("Something went wrong");
            }
        }

        public async Task<Response<List<SportLevelsResponseDto>>> GetAllSportLevelByUserId(string userId, SportLevelSearchDto searchDto)
        {
            try
            {
                if(string.IsNullOrEmpty(userId))
                {
                    return new ResponseHandler().BadRequest<List<SportLevelsResponseDto>>("User ID is required");
                }

                var search = searchDto.SearchTerm?.Trim().ToLower() ?? string.Empty;

                Expression<Func<MemberSportLevel, bool>> criteria = m =>
                    m.UserId == userId &&
                    (string.IsNullOrEmpty(search) ||
                     (m.SportName != null && m.SportName.ToLower().Contains(search)) ||
                     (m.SportNameAr != null && m.SportNameAr.ToLower().Contains(search)));


                Expression<Func<MemberSportLevel, SportLevelsResponseDto>> selector = m => new SportLevelsResponseDto
                {
                    SportId = m.SportId,
                    SportName = m.SportName,
                    SportNameAr = m.SportNameAr,
                    SportImageUrl = m.SportImage,
                    CreationDate = m.CreationDate,
                    TenantId = m.TenantId
                };

                var sportLevels = await unitOfWork.Repository<MemberSportLevel>()
                    .GetAllWithSpecificationsAndProjection(new GeneralSpecifications<MemberSportLevel>(criteria), selector);

                Log.Information("Retrieved {Count} sport levels for user ID {UserId}", sportLevels.Count, userId);

                var uniqueSports = sportLevels
                    .GroupBy(ul =>  ul.SportId)
                    .Select(g => g.OrderByDescending(ul => ul.CreationDate).First())
                    .OrderBy(ul => ul.SportName)
                    .ToList();


                var tasks = sportLevels.Select(async r =>
                {
                    r.SportImageUrl = await DocumentSetting.GetPreSignedUrlAsync(r.SportImageUrl);

                });
                await Task.WhenAll(tasks);

                Log.Information("Successfully processed sport images for user ID {UserId}", userId);

                return new ResponseHandler().Success(uniqueSports);


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting sport levels by user ID: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<List<SportLevelsResponseDto>>("Something went wrong");
            }
        }

        public async Task<Response<List<SportLevelTenantResponseDto>>> GetAllSportLevelTenantsByUserId(string userId,int sportId, SportLevelTenantSearchDto searchDto)
        {
            try
            {

                if(string.IsNullOrEmpty(userId))
                {
                    return new ResponseHandler().BadRequest<List<SportLevelTenantResponseDto>>("User ID is required");
                }


                if(sportId <= 0)
                {
                    return new ResponseHandler().BadRequest<List<SportLevelTenantResponseDto>>("Sport ID is required and must be greater than zero");
                }

                Expression<Func<MemberSportLevel, bool>> criteria = m =>
                    m.UserId == userId &&
                    m.SportId == sportId &&
                    (string.IsNullOrEmpty(searchDto.SearchTerm) ||
                     (m.Tenant.Name != null && m.Tenant.Name.ToLower().Contains(searchDto.SearchTerm.ToLower()))); 

                Expression<Func<MemberSportLevel, SportLevelTenantResponseDto>> selector = m => new SportLevelTenantResponseDto
                {
                    TenantId = m.TenantId,
                    TenantName = m.Tenant.Name,
                    TenantImageUrl = m.Tenant.Image
                };


                var tenants = await unitOfWork.Repository<MemberSportLevel>()
                    .GetAllWithSpecificationsAndProjection(new GeneralSpecifications<MemberSportLevel>(criteria), selector);

                var uniqueTenants = tenants
                    .GroupBy(t => t.TenantId)
                    .Select(g => g.First())
                    .OrderBy(t => t.TenantName)
                    .ToList();


                Log.Information("Retrieved {Count} sport level tenants for user ID {UserId} and sport ID {SportId}", uniqueTenants.Count, userId, sportId);

                return new ResponseHandler().Success(uniqueTenants);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting sport level tenants by user ID: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<List<SportLevelTenantResponseDto>>("Something went wrong");
            }
            
        }
    }
}
