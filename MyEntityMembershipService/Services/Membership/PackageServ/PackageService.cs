using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos.MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Membership.PackageServ
{
    public class PackageService:IPackageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;


        public PackageService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork=unitOfWork;
            this.mapper=mapper;
            this.configuration=configuration;
            this.httpContextAccessor=httpContextAccessor;
        }

        public async Task<Response<PackageDto>> AddPackage(AddPackageWithSubPackagesDto dto)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                ValidatePackage(dto);
                PrepareMembers(dto);

                var package = await CreatePackage(dto);

                await HandleScopes(package.Id, dto.PackageScopeTypes); 

                var venues = await HandleAccess(package.Id, dto);
                await HandleProducts(package.Id, dto.Products);
                var authIds = await HandleAuthorizations(package.Id, dto.RequestAuthorizationIds);
                await HandleSubPackages(package, dto, venues, authIds);

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

        private async Task HandleScopes(int packageId, List<PackageScopeType> scopeTypes)
        {
            foreach (var scopeType in scopeTypes.Distinct())
            {
                var scope = await unitOfWork.Repository<PackageScope>()
                    .FindAsync(s => s.ScopeType == scopeType);

                if (scope == null)
                    throw new Exception($"PackageScope '{scopeType}' not found in database");

                await unitOfWork.Repository<PackageScopeAssignment>().Add(new PackageScopeAssignment
                {
                    PackageId = packageId,
                    PackageScopeId = scope.Id
                });
            }

            await unitOfWork.Save();
        }

        private void ValidatePackage(AddPackageWithSubPackagesDto dto)
        {
            if (!dto.PackageScopeTypes.Any())
                throw new Exception("Package must have at least one scope");

            var isAccess = dto.PackageScopeTypes.Contains(PackageScopeType.Access);
            var isEligibility = dto.PackageScopeTypes.Contains(PackageScopeType.Eligibility);

            if (isAccess && !dto.VenueIds.Any())
                throw new Exception("Access package must have venues");

            if (isEligibility && !isAccess && dto.VenueIds.Any())
                throw new Exception("Eligibility-only package cannot have venues");
        }

        private void PrepareMembers(AddPackageWithSubPackagesDto dto)
        {
            dto.SubMemberCategories ??= new List<SubMemberCategoryDto>();

            if (dto.IsMainMember == MembershipMode.FullMembership)
            {
                var hasMainMember = dto.SubMemberCategories
                    .Any(x => x.MemberTypeId == 5);

                if (!hasMainMember)
                {
                    dto.SubMemberCategories.Add(new SubMemberCategoryDto
                    {
                        MemberTypeId = 5,
                        MemberTypeName = "MainMember",
                        EnglishTitle = "MainMember",
                        ArabicTitle = "العضو الرئيسي",
                        Price = 0
                    });
                }
            }
            else
            {
                dto.SubMemberCategories.Clear();
            }
        }

        private async Task<Package> CreatePackage(AddPackageWithSubPackagesDto dto)
        {
            var tenantClaim = httpContextAccessor.HttpContext?
                .User?
                .FindFirst("tenantId")?.Value;

            if (!int.TryParse(tenantClaim, out var tenantId))
                throw new UnauthorizedAccessException("Invalid tenant from token");

            var package = mapper.Map<Package>(dto);

            package.PackageType = dto.Visibility ?? Data.Entity.Entities.Enums.PackageType.Public;
            package.TenantId = tenantId;

            if (dto.ImageUrl != null)
            {
                package.Image = await DocumentSetting.AddToContainerAndGetUrl(
                    configuration["StorageConnectionString"],
                    "buildupimagedocs",
                    dto.ImageUrl);
            }

            await unitOfWork.Repository<Package>().Add(package);
            await unitOfWork.Save();

            return package;
        }

        private async Task<List<NewVenue>> HandleAccess(int packageId, AddPackageWithSubPackagesDto dto)
        {
            if (!dto.PackageScopeTypes.Contains(PackageScopeType.Access))
                return new List<NewVenue>();

            var spec = new GeneralSpecifications<NewVenue>(v => dto.VenueIds.Contains(v.Id))
            {
                Includes =
                [
                    v => v.Include(x => x.Facility)
                  .ThenInclude(f => f.Branch)
                ]
            };

            var venues = await unitOfWork.Repository<NewVenue>()
                .GetAllWithSpecifications(spec);

            var branches = new HashSet<int>();
            var facilities = new HashSet<int>();

            foreach (var v in venues)
            {
                var branchId = v.Facility.BranchId;
                var facilityId = v.FacilityId;

                if (branches.Add(branchId))
                {
                    await unitOfWork.Repository<AccessedBranches>().Add(new AccessedBranches
                    {
                        PackageId = packageId,
                        BranchId = branchId
                    });
                }

                if (facilities.Add(facilityId))
                {
                    await unitOfWork.Repository<AccessedBranchesFacilities>().Add(new AccessedBranchesFacilities
                    {
                        PackageId = packageId,
                        BranchId = branchId,
                        FacilityId = facilityId
                    });
                }

                await unitOfWork.Repository<AccessedFacilitiesVenues>().Add(new AccessedFacilitiesVenues
                {
                    PackageId = packageId,
                    BranchId = branchId,
                    FacilityId = facilityId,
                    VenueId = v.Id
                });
            }

            await unitOfWork.Save();

            return venues;
        }

        private async Task HandleProducts(int packageId, List<PackageProductDto>? products)
        {
            if (products == null || !products.Any())
                return;

            foreach (var p in products)
            {
                await unitOfWork.Repository<PackageProduct>().Add(new PackageProduct
                {
                    PackageId = packageId,
                    ProductId = p.ProductId,

                    MaxSessions = p.MaxSessions,
                    SessionInterval = p.SessionInterval,
                    PerInterval = p.PerInterval
                });
            }

            await unitOfWork.Save();
        }
        private async Task<List<int>> HandleAuthorizations(int packageId, List<int>? authIds)
        {
            var ids = authIds ?? new List<int>();
            ids.Add(6);

            var distinctIds = ids.Distinct().ToList();

            foreach (var id in distinctIds)
            {
                await unitOfWork.Repository<PackageRequestAuthourizations>().Add(new PackageRequestAuthourizations
                {
                    PackageId = packageId,
                    RequestAuthourizationId = id
                });
            }

            await unitOfWork.Save();

            return distinctIds;
        }

        private async Task HandleSubPackages(
            Package package,
            AddPackageWithSubPackagesDto dto,
            List<NewVenue> parentVenues,
            List<int> authIds)
        {
            if (dto.SubPackages == null) return;

            foreach (var subDto in dto.SubPackages)
            {
                ValidateSubPackage(dto, subDto);

                var subPackage = mapper.Map<Package>(subDto);

                subPackage.SuperPackageId = package.Id;
                subPackage.TenantId = package.TenantId;
                subPackage.MembershipMode = dto.IsMainMember;

                await unitOfWork.Repository<Package>().Add(subPackage);
                await unitOfWork.Save();

                // inherit parent scopes instead of hardcoding Eligibility
                await InheritScopes(subPackage.Id, package.Id);

                await HandleSubAccess(subPackage.Id, subDto, parentVenues, dto);
                await HandleProducts(subPackage.Id, subDto.Products ?? dto.Products);
                await InheritMembers(subPackage.Id, package.Id);
                await InheritAuthorizations(subPackage.Id, package.Id);
            }
        }

        private async Task InheritScopes(int subPackageId, int superPackageId)
        {
            var parentScopes = await unitOfWork.Repository<PackageScopeAssignment>()
                .GetAllAsync(x => x.PackageId == superPackageId);

            foreach (var scope in parentScopes)
            {
                await unitOfWork.Repository<PackageScopeAssignment>().Add(new PackageScopeAssignment
                {
                    PackageId = subPackageId,
                    PackageScopeId = scope.PackageScopeId
                });
            }

            await unitOfWork.Save();
        }
        private void ValidateSubPackage(AddPackageWithSubPackagesDto dto, SubPackageDto subDto)
        {
            var invalidVenues = subDto.VenueIds.Except(dto.VenueIds);
            if (invalidVenues.Any())
                throw new Exception("Invalid sub package venues");

            var parentProducts = dto.Products?.Select(p => p.ProductId).ToList() ?? new List<int>();
            var subProducts = subDto.Products?.Select(p => p.ProductId).ToList() ?? new List<int>();

            if (subProducts.Any() && !parentProducts.Any())
                throw new Exception("Sub package cannot have products if parent has none");

            var invalidProducts = subProducts.Except(parentProducts);
            if (invalidProducts.Any())
                throw new Exception("Invalid sub package products");
        }
        private async Task HandleSubAccess(
            int packageId,
            SubPackageDto subDto,
            List<NewVenue> parentVenues,
            AddPackageWithSubPackagesDto dto)
        {
            if (!dto.PackageScopeTypes.Contains(PackageScopeType.Access)) return;

            var venues = parentVenues.Where(v => subDto.VenueIds.Contains(v.Id)).ToList();
            var branches = new HashSet<int>();
            var facilities = new HashSet<int>();

            foreach (var v in venues)
            {
                var branchId = v.Facility.BranchId;
                var facilityId = v.FacilityId;

                if (branches.Add(branchId))
                {
                    await unitOfWork.Repository<AccessedBranches>().Add(new AccessedBranches
                    {
                        PackageId = packageId,
                        BranchId = branchId
                    });
                }

                if (facilities.Add(facilityId))
                {
                    await unitOfWork.Repository<AccessedBranchesFacilities>().Add(new AccessedBranchesFacilities
                    {
                        PackageId = packageId,
                        BranchId = branchId,
                        FacilityId = facilityId
                    });
                }

                await unitOfWork.Repository<AccessedFacilitiesVenues>().Add(new AccessedFacilitiesVenues
                {
                    PackageId = packageId,
                    BranchId = branchId,
                    FacilityId = facilityId,
                    VenueId = v.Id
                });
            }

            await unitOfWork.Save();
        }
        private async Task InheritMembers(int subPackageId, int superPackageId)
        {
            var superMembers = await unitOfWork.Repository<SubMemberCategory>()
                .GetAllAsync(x => x.PackageId == superPackageId);

            foreach (var m in superMembers)
            {
                await unitOfWork.Repository<SubMemberCategory>().Add(new SubMemberCategory
                {
                    PackageId = subPackageId,
                    MemberTypeId = m.MemberTypeId,
                    EnglishTitle = m.EnglishTitle,
                    ArabicTitle = m.ArabicTitle,
                    Price = m.Price                 
                });
            }

            await unitOfWork.Save();
        }
        private async Task InheritAuthorizations(int subPackageId, int superPackageId)
        {
            var superAuths = await unitOfWork.Repository<PackageRequestAuthourizations>()
                .GetAllAsync(x => x.PackageId == superPackageId);
                

            foreach (var auth in superAuths)
            {
                await unitOfWork.Repository<PackageRequestAuthourizations>().Add(new PackageRequestAuthourizations
                {
                    PackageId = subPackageId,
                    RequestAuthourizationId = auth.RequestAuthourizationId
                });
            }

            await unitOfWork.Save();
        }

        public async Task<Response<PaginationToReturnDto<GetAllPackageResponseDto>>> GetAllPackagesAsync(GetAllPackageFilterResponseDto filter)
        {
            try
            {
                if (!filter.TenantId.HasValue)
                {
                    Log.Warning("GetAllPackagesAsync called without TenantId");
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<GetAllPackageResponseDto>>("TenantId is required");
                }

                Expression<Func<Package, bool>> criteria = p =>
                p.TenantId == filter.TenantId.Value &&
                p.SuperPackageId == null &&
                (string.IsNullOrEmpty(filter.SearchTerm) ||
                p.Name.Trim().ToLower().Contains(filter.SearchTerm.Trim().ToLower()));

                Expression<Func<Package, GetAllPackageResponseDto>> selector = p =>
                new GetAllPackageResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Duration = p.Duration,
                    PackageType = p.PackageType,
                    Image = p.Image,
                    PackageScopes = p.PackageScopeAssignments
                    .Select(x => x.PackageScope.ScopeType)
                    .ToList(),
                    MembersCount = p.Memberships.Count(m => !m.IsDeleted)
                };

                var totalCount = await unitOfWork
                .Repository<Package>()
                .GetCountByCriteriaAsync(criteria);

                var spec = new GeneralSpecifications<Package>(criteria, filter.PageNumber, filter.PageSize) { AsNoTracking = true };


                spec.SetOrderByDescending(x => x.Id);

                var data = await unitOfWork
                    .Repository<Package>()
                    .GetAllWithSpecificationsAndProjection(spec, selector);


                var dataImagesTasks = data.Select(async p =>
                {
                    p.Image = await DocumentSetting.GetPreSignedUrlAsync(p.Image);
                });

                await Task.WhenAll(dataImagesTasks);

                var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

                var pagination = new PaginationToReturnDto<GetAllPackageResponseDto>(
                    count: totalCount,
                    totalPages: totalPages,
                    pageSize: filter.PageSize,
                    pageNumber: filter.PageNumber,
                    data: data
                );

                Log.Information("GetAllPackagesAsync successfully retrieved {Count} packages for TenantId {TenantId}", totalCount, filter.TenantId);

                return new ResponseHandler().Success(pagination);

            }
            catch (Exception ex)
            {
                Log.Error("Error in GetAllPackagesAsync: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<PaginationToReturnDto<DTOs.Membership.PackageDtos.GetAllPackageResponseDto>>("Something went wrong");
            }
        }

        public async Task<Response<string>> UpdatePackageActivationStatusAsync(UpdatePackageActivitionDto dto)
        {

            try
            {
                if (!dto.TenantId.HasValue)
                {
                    Log.Warning("UpdatePackageActivationStatus failed — TenantId is missing.");
                    return new ResponseHandler().BadRequest<string>("TenantId is required.");
                }

                var package = await unitOfWork.Repository<Package>().FindAsync(p => p.Id == dto.PackageId && p.TenantId == dto.TenantId.Value);

                if (package is null)
                {
                    Log.Warning("UpdatePackageActivationStatus failed — package not found. PackageId: {PackageId}", dto.PackageId);
                    return new ResponseHandler().NotFound<string>("Package not found.");
                }

                if (package.IsActive == dto.IsActive)
                {
                    var status = dto.IsActive ? "active" : "inactive";
                    Log.Warning("UpdatePackageActivationStatus — no change needed. PackageId: {PackageId} is already {Status}", dto.PackageId, status);
                    return new ResponseHandler().BadRequest<string>($"Package is already {status}.");
                }

                if (dto.IsActive)
                {
                    var validationError = await ValidatePackageConfigurationsAsync(dto.PackageId);
                    if (validationError is not null)
                    {
                        Log.Warning("UpdatePackageActivationStatus failed — missing configuration. PackageId: {PackageId} | {Error}", dto.PackageId, validationError);
                        return new ResponseHandler().BadRequest<string>(validationError);
                    }
                }

                var previousStatus = package.IsActive;
                package.IsActive = dto.IsActive;

                unitOfWork.Repository<Package>().Update(package);
                await unitOfWork.Save();

                Log.Information(
                    "UpdatePackageActivationStatus succeeded. PackageId: {PackageId} | {PreviousStatus} → {NewStatus}",
                    dto.PackageId, previousStatus, dto.IsActive);

                var message = dto.IsActive ? "Package activated successfully." : "Package deactivated successfully.";
                return new ResponseHandler().Success(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdatePackageActivationStatus threw an exception. PackageId: {PackageId}", dto.PackageId);
                return new ResponseHandler().BadRequest<string>("An unexpected error occurred.");
            }
        }

        public async Task<Response<PackageResponseDto>> GetPackageByIdAsync(int packageId, int? tenantId)
        {
            try
            {
                if (!tenantId.HasValue)
                {
                    Log.Warning("GetPackageByIdAsync failed — TenantId is missing. PackageId: {PackageId}", packageId);
                    return new ResponseHandler().BadRequest<PackageResponseDto>("TenantId is required.");
                }


                Expression<Func<Package, bool>> criteria = p => p.Id == packageId && p.TenantId == tenantId.Value;

                Expression<Func<Package, PackageResponseDto>> selector = p => new PackageResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Duration = p.Duration,
                    Image = p.Image,
                    IsActive = p.IsActive,
                    PackageType = p.PackageType,
                    PackageScopes = p.PackageScopeAssignments.Select(x => x.PackageScope.ScopeType).ToList()
                };


                var packageSpec = new GeneralSpecifications<Package>(criteria) { AsNoTracking = true };

                var package = await unitOfWork.Repository<Package>().GetWithSpecificationsAndProjection(packageSpec, selector);
                if (package is null)
                {
                    Log.Warning("GetPackageByIdAsync failed — package not found. PackageId: {PackageId}", packageId);
                    return new ResponseHandler().NotFound<PackageResponseDto>("Package not found.");
                }


                package.Image = await DocumentSetting.GetPreSignedUrlAsync(package.Image);

                Log.Information("GetPackageByIdAsync successfully retrieved package. PackageId: {PackageId}", packageId);

                return new ResponseHandler().Success(package);


            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetPackageByIdAsync threw an exception. PackageId: {PackageId}", packageId);
                return new ResponseHandler().BadRequest<PackageResponseDto>("An unexpected error occurred.");
            }
            
        }

        public async Task<Response<PaginationToReturnDto<PackageMemberDto>>> GetAllPackageMemberAsync(
            int? tenantId, int packageId, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                if (!tenantId.HasValue)
                {
                    Log.Warning("GetAllPackageMemberAsync failed — TenantId is missing. PackageId: {PackageId}", packageId);
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<PackageMemberDto>>("TenantId is required.");
                }

                Expression<Func<Data.Membership.Entities.Membership, bool>> criteria = m =>
                    m.PackageId == packageId &&
                    m.TenantId == tenantId.Value &&
                    !m.IsDeleted;

                Expression<Func<Data.Membership.Entities.Membership, PackageMemberDto>> selector = m => new PackageMemberDto
                {
                    UserId = m.UserId,
                    Name = (m.FirstName + " " + m.LastName).Trim(),
                    MobileNumber = m.PhoneNumber,
                    JoinDate = m.StartDate.Value,
                    ExpiryDate = m.EndDate.Value,
                    Email = m.Email
                };

                var totalCount = await unitOfWork
                    .Repository<Data.Membership.Entities.Membership>()
                    .GetCountByCriteriaAsync(criteria);

                var spec = new GeneralSpecifications<Data.Membership.Entities.Membership>(criteria, pageNumber, pageSize)
                {
                    AsNoTracking = true
                };

                spec.SetOrderByDescending(x => x.Id);

                var data = await unitOfWork
                    .Repository<Data.Membership.Entities.Membership>()
                    .GetAllWithSpecificationsAndProjection(spec, selector);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagination = new PaginationToReturnDto<PackageMemberDto>(
                    count: totalCount,
                    totalPages: totalPages,
                    pageSize: pageSize,
                    pageNumber: pageNumber,
                    data: data
                );

                Log.Information(
                    "GetAllPackageMemberAsync retrieved {Count} members for PackageId {PackageId}",
                    totalCount, packageId);

                return new ResponseHandler().Success(pagination);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllPackageMemberAsync threw an exception. PackageId: {PackageId}", packageId);
                return new ResponseHandler().BadRequest<PaginationToReturnDto<PackageMemberDto>>("An unexpected error occurred.");
            }
        }

        private async Task<string?> ValidatePackageConfigurationsAsync(int packageId)
        {

            var packageAuthorizations = await unitOfWork
                .Repository<PackageRequestAuthourizations>()
                .GetAllAsync(x => x.PackageId == packageId);

            var requestTypeIds = packageAuthorizations
                .Select(x => x.RequestAuthourizationId)
                .Distinct()
                .ToList();

            if (!requestTypeIds.Any())
                return null;

            var configurationChecks = new Dictionary<int, Func<Task<bool>>>
            {
                [6] = async () => await unitOfWork
                    .Repository<NewMembershipConfiguration>()
                    .FindAsync(c => c.PackageId == packageId) is not null,

                [7] = async () => await unitOfWork
                    .Repository<RenewalConfiguration>()
                    .FindAsync(c => c.PackageId == packageId) is not null,

                [12] = async () => await unitOfWork
                    .Repository<FreezeConfiguration>()
                    .FindAsync(c => c.PackageId == packageId) is not null,

                [16] = async () => await unitOfWork
                    .Repository<AdditionalConfiguration>()
                    .FindAsync(c => c.PackageId == packageId) is not null,

                [17] = async () => await unitOfWork
                    .Repository<CancellationConfiguration>()
                    .FindAsync(c => c.PackageId == packageId) is not null,

                [18] = async () => await unitOfWork
                    .Repository<RemovalConfiguration>()
                    .FindAsync(c => c.PackageId == packageId) is not null,
            };

            var configurationNames = new Dictionary<int, string>
            {
                [6] = "New Membership",
                [7] = "Renewal",
                [12] = "Freeze",
                [16] = "Additional Member",
                [17] = "Cancellation",
                [18] = "Removal",
            };

            foreach (var requestTypeId in requestTypeIds)
            {
                if (!configurationChecks.TryGetValue(requestTypeId, out var check))
                    continue; 

                var configExists = await check();
                if (!configExists)
                {
                    var name = configurationNames[requestTypeId];
                    return $"Package cannot be activated. '{name}' configuration is missing.";
                }
            }

            return null;
        }

        public async Task<Response<PaginationToReturnDto<ActiveRequestResponseDto>>> GetPackageActiveRequestsAsync(int packageId, ActiveRequestFilter filter, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                if (!filter.TenantId.HasValue)
                {
                    Log.Warning("GetPackageActiveRequestsAsync failed — TenantId is missing.");
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<ActiveRequestResponseDto>>("TenantId is required.");
                }


                Expression<Func<MembershipRequest, bool>> criteria = r =>
                r.TenantId == filter.TenantId.Value &&
                r.PackageId == packageId &&

                (string.IsNullOrEmpty(filter.SearchTerm) ||
                (r.FirstName != null && r.FirstName.Trim().ToLower().Contains(filter.SearchTerm.Trim().ToLower())) ||
                (r.LastName != null && r.LastName.Trim().ToLower().Contains(filter.SearchTerm.Trim().ToLower()))) &&

                (!filter.RequestTypeFilterId.HasValue || r.RequestTypeId == filter.RequestTypeFilterId.Value) &&
                (!filter.StatusFilter.HasValue || r.Status == filter.StatusFilter.Value) &&
                (!filter.PaymentStatusFilter.HasValue || r.RequestPaymentStatus == filter.PaymentStatusFilter.Value) &&
                (!filter.PaymentProceduresTypeFilter.HasValue || r.PaymentProceduresType == filter.PaymentProceduresTypeFilter.Value);


                Expression<Func<MembershipRequest, ActiveRequestResponseDto>> selector = r => new ActiveRequestResponseDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    FirstName = r.FirstName,
                    SecondName = r.SecondName,
                    MemberType = r.MemberType != null ? r.MemberType.Name : null,
                    RequestType = r.RequestType != null ? r.RequestType.Name : null,
                    SubmittedAt = r.CreationDate.Value, 
                    PaymentProceduresType = r.PaymentProceduresType,
                    Status = r.Status,
                    PaymentStatus = r.RequestPaymentStatus
                };


                var totalCount = await unitOfWork
                .Repository<MembershipRequest>()
                .GetCountByCriteriaAsync(criteria);


                var spec = new GeneralSpecifications<MembershipRequest>(criteria, pageNumber, pageSize)
                {
                    AsNoTracking = true
                };

                spec.SetOrderByDescending(x => x.Id);

                var data = await unitOfWork
                    .Repository<MembershipRequest>()
                    .GetAllWithSpecificationsAndProjection(spec, selector);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagination = new PaginationToReturnDto<ActiveRequestResponseDto>(
                count: totalCount,
                totalPages: totalPages,
                pageSize: pageSize,
                pageNumber: pageNumber,
                data: data
                );

                Log.Information(
                    "GetPackageActiveRequestsAsync retrieved {Count} requests for TenantId {TenantId}",
                    totalCount, filter.TenantId);

                return new ResponseHandler().Success(pagination);

            }
            catch (Exception ex) {
                Log.Error(ex, "GetPackageActiveRequestsAsync threw an exception. TenantId: {TenantId}", filter.TenantId);
                return new ResponseHandler().BadRequest<PaginationToReturnDto<ActiveRequestResponseDto>>("An unexpected error occurred.");
            }
        }


        /// <summary>
        /// Get members available for a specific request type.
        /// Used for Freeze/Addition where user must select a member.
        /// </summary>

        public async Task<Response<List<DTOs.Entity.PackageDtos.SubMemberCategoryDto>>> GetAvailableMembersAsync(DTOs.Entity.PackageDtos.GetAvailableMembersRequest request)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();
                var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

                // 1. Validate request type
                var requestType = await unitOfWork
                    .Repository<RequestType>()
                    .GetByIdAsync(request.RequestTypeId);

                if (requestType == null)
                {
                    return new ResponseHandler()
                        .BadRequest<List<DTOs.Entity.PackageDtos.SubMemberCategoryDto>>(
                            "Request type not found");
                }

                // Parent or main member request does not need member selection in parent ,new membership , cancellation
                if (requestType.Id == 21 || requestType.Id == 6 || requestType.Id == 17)
                {
                    return new ResponseHandler()
                        .BadRequest<List<DTOs.Entity.PackageDtos.SubMemberCategoryDto>>(
                            "Member selection not needed for this request type");
                }

                // 2. Validate active main membership
                var hasMainMembership = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetWithSpecifications(
                    new GeneralSpecifications<Data.Membership.Entities.Membership>
                    (x => x.UserId == userId && x.MembershipStatus==MembershipStatus.Active 
                    && x.EndDate > DateTime.UtcNow 
                    && x.TenantId == tenantId
                    &&x.IsMainMembership==true 
                    &&x.PackageId==request.PackageId));

                if (hasMainMembership == null)
                {
                    return new ResponseHandler()
                        .BadRequest<List<DTOs.Entity.PackageDtos.SubMemberCategoryDto>>(
                            "You must be an active Main member to perform this action");
                }

                // 3. Get available sub-member categories
                var spec = new GeneralSpecifications<SubMemberCategory>(
                    x => x.PackageId == request.PackageId
                );

                spec.AddInclude(q => q.Include(x => x.MemberType));

                var subMemberCategories = await unitOfWork
                    .Repository<SubMemberCategory>()
                    .GetAllWithSpecifications(spec);

                // 4. Map result
                var result = subMemberCategories.Select(x => new DTOs.Entity.PackageDtos.SubMemberCategoryDto
                {
                    MemberTypeId = x.MemberTypeId,
                    MemberTypeName = x.MemberType.Name,
                    ArabicTitle = x.ArabicTitle,
                    EnglishTitle = x.EnglishTitle,
                    Price = x.Price
                }).ToList();

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting available members");

                return new ResponseHandler()
                    .BadRequest<List<DTOs.Entity.PackageDtos.SubMemberCategoryDto>>(
                        "Something went wrong");
            }
        }
        private string GenerateAvatar(string firstName, string lastName)
        {
            var initials = $"{(firstName?.FirstOrDefault() ?? 'U')}{(lastName?.FirstOrDefault() ?? 'S')}";
            return initials.ToUpper();
        }

        public async Task<Response<List<SubPackageResponsDto>>> GetAllSubPackagesAsync(int? tenantId, int packageId)
        {
            try
            {
                if(!tenantId.HasValue)
                {
                    Log.Warning("GetAllSubPackagesAsync failed — TenantId is missing. PackageId: {PackageId}", packageId);
                    return new ResponseHandler().BadRequest<List<SubPackageResponsDto>>("TenantId is required.");
                }

                Expression<Func<Package, bool>> criteria = p => p.SuperPackageId == packageId && p.TenantId == tenantId.Value;

                Expression<Func<Package, SubPackageResponsDto>> selector = p => new SubPackageResponsDto
                {
                    Id = p.Id,
                    SuperPackageId = p.SuperPackageId.Value,
                    Name = p.Name,
                    Duration = p.Duration,
                    PackageType = p.PackageType,
                    PackageScopes = p.PackageScopeAssignments.Select(x => x.PackageScope.ScopeType).ToList(),
                    ProductDetails = p.PackageProducts.Select(pp => new SubPackageProduct
                    {
                        MaxSessions = pp.MaxSessions,
                        SessionInterval = pp.SessionInterval,
                        PerInterval = pp.PerInterval
                    }).ToList(),
                    ProductTypes = p.PackageProducts.Select(pp => new SubPackageProductType
                    {
                        Name = pp.Product.Name,
                        ProductId = pp.Product.ProductId,
                        ProductType = pp.Product.ProductType

                    }).ToList()
                };
                var subPackagesSpec = new GeneralSpecifications<Package>(criteria) { AsNoTracking = true };
                var subPackages = await unitOfWork.Repository<Package>().GetAllWithSpecificationsAndProjection(subPackagesSpec,selector);

                Log.Information("GetAllSubPackagesAsync successfully retrieved {Count} sub-packages for PackageId {PackageId}", subPackages.Count, packageId);


                return new ResponseHandler().Success(subPackages);


            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllSubPackagesAsync threw an exception. PackageId: {PackageId}", packageId);
                return new ResponseHandler().BadRequest<List<SubPackageResponsDto>>("An unexpected error occurred.");
            }
            
        }

        public async Task<Response<PackageBenefitsDto>> GetPackageBenefitsAsync(int? tenantId, int packageId)
        {
            try
            {
                if (!tenantId.HasValue)
                {
                    Log.Warning("GetPackageBenefitsAsync — TenantId missing. PackageId: {PackageId}", packageId);
                    return new ResponseHandler().BadRequest<PackageBenefitsDto>("TenantId is required.");
                }

                var spec = new GeneralSpecifications<Package>(
                    p => p.Id == packageId && p.TenantId == tenantId.Value
                )
                {
                    AsNoTracking = true ,
                    AsSplitQuery = true
                };

                spec.AddInclude(q => q
                    .Include(p => p.Branches!)
                        .ThenInclude(b => b.Branch)
                    .Include(p => p.Facilities!)
                        .ThenInclude(f => f.Branch)
                    .Include(p => p.Facilities!)
                        .ThenInclude(f => f.Facility)
                    .Include(p => p.Venues!)
                        .ThenInclude(v => v.Facility)
                    .Include(p => p.Venues!)
                        .ThenInclude(v => v.Venue)
                    .Include(p => p.PackageProducts)
                        .ThenInclude(pp => pp.Product)  
                );

                var package = await unitOfWork.Repository<Package>()
                    .GetWithSpecifications(spec);  

                if (package is null)
                {
                    Log.Warning("GetPackageBenefitsAsync — Package {PackageId} not found for Tenant {TenantId}",
                        packageId, tenantId.Value);
                    return new ResponseHandler().NotFound<PackageBenefitsDto>("Package not found.");
                }


                var dto = new PackageBenefitsDto
                {
                    PackageAccesses = package.Branches!
                        .Select(ab => new PackageAccess
                        {
                            BranchId = ab.BranchId,
                            BranchName = ab.Branch.Name,
                            BranchImage = ab.Branch.BranchImageURL,

                            PackageFacilityAccesses = package.Facilities!
                                .Where(f => f.BranchId == ab.BranchId)
                                .Select(f => new PackageFacilityAccess
                                {
                                    FacilityId = f.FacilityId,
                                    FacilityName = f.Facility.FacilityName,
                                    FacilityImage = f.Facility.FacilityImageUrl,

                                    PackageVenuesAccesses = package.Venues!
                                        .Where(v => v.BranchId == ab.BranchId
                                                 && v.FacilityId == f.FacilityId)
                                        .Select(v => new PackageVenuesAccess
                                        {
                                            VenueId = v.VenueId,
                                            VenueName = v.Venue.Name,
                                            VenueImage = v.Venue.VenueImageURL
                                        }).ToList()
                                }).ToList()
                        }).ToList(),

                    PackageEligibilities = package.PackageProducts
                        .Select(pp => new PackageEligibility
                        {
                            ProductId = pp.Product.Id,
                            ProductType = pp.Product.ProductType,
                            Name = pp.Product.Name,
                            Image = pp.Product.Image,
                            StartDate = pp.Product.StartDate,
                            EndDate = pp.Product.EndDate
                        }).ToList()
                };

                var tasks = dto.PackageAccesses.Select(async a => {
                    a.BranchImage = await DocumentSetting.GetPreSignedUrlAsync(a.BranchImage);
                })
                    .Concat(dto.PackageAccesses.SelectMany(a => a.PackageFacilityAccesses).Select(async f => {
                        f.FacilityImage = await DocumentSetting.GetPreSignedUrlAsync(f.FacilityImage);
                    }))
                    .Concat(dto.PackageAccesses.SelectMany(a => a.PackageFacilityAccesses)
                        .SelectMany(f => f.PackageVenuesAccesses).Select(async v => {
                            v.VenueImage = await DocumentSetting.GetPreSignedUrlAsync(v.VenueImage);
                        }))
                    .Concat(dto.PackageEligibilities.Select(async e => {
                        e.Image = await DocumentSetting.GetPreSignedUrlAsync(e.Image);
                    }));

                await Task.WhenAll(tasks);


                Log.Information(
                    "GetPackageBenefitsAsync success. PackageId: {PackageId} | Branches: {B} | Eligibilities: {E}",
                    packageId, dto.PackageAccesses.Count, dto.PackageEligibilities.Count);

                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetPackageBenefitsAsync threw. PackageId: {PackageId}", packageId);
                return new ResponseHandler().BadRequest<PackageBenefitsDto>("An unexpected error occurred.");
            }
        }
    }
}
