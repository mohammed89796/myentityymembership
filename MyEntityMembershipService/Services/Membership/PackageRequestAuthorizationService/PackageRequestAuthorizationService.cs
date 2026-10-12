using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.FormRepo;
using MyEntityMembershipService.Repositories.Membership.PackageRequestAuthourizationsRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.PackageRequestAuthorizationService
{
    public class PackageRequestAuthorizationService : IPackageRequestAuthorizationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IPackageRequestAuthorizationRepository repository;
        private readonly IFormRepository formRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PackageRequestAuthorizationService(IUnitOfWork unitOfWork, IMapper mapper, IPackageRequestAuthorizationRepository repository, IFormRepository formRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.repository = repository;
            this.formRepository = formRepository;
            this.httpContextAccessor=httpContextAccessor;
        }

        public async Task<Response<PackageRequestAuthorizationDto>> AddPackageRequestAuthorization(PackageRequestAuthorizationDto packageRequestAuthorizationDto)
        {
            try
            {
                var packageRequestAuthorization = mapper.Map<PackageRequestAuthourizations>(packageRequestAuthorizationDto);
                await unitOfWork.Repository<PackageRequestAuthourizations>().Add(packageRequestAuthorization);
                await unitOfWork.Save();
                packageRequestAuthorizationDto = mapper.Map<PackageRequestAuthorizationDto>(packageRequestAuthorization);
                return new ResponseHandler().Success(packageRequestAuthorizationDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PackageRequestAuthorizationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<PackageRequestAuthorizationDto>>> GetAllPackageRequestAuthorizations()
        {
            try
            {
                var packageRequestAuthorizations = await unitOfWork.Repository<PackageRequestAuthourizations>().GetAllAsync();
                var packageRequestAuthorizationDtos = mapper.Map<List<PackageRequestAuthorizationDto>>(packageRequestAuthorizations);
                return new ResponseHandler().Success(packageRequestAuthorizationDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<PackageRequestAuthorizationDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<PackageMmbertypeFormsAndConfigrations>> GetPackageRequestAuthorizationById(int id)
        {
            try
            {
                var packageRequestAuthorization = await repository.GetAuthorizationsByPackageIdAsync(id);
                var packageRequestAuthorizationDto = mapper.Map<List<PackageRequestAuthorizationDto>>(packageRequestAuthorization);

                var packageMemberType = await repository.GetMemberTypesWithRequestTypesAndForms(id);
                var packageMemberTypeDto = mapper.Map<List<PackageMemberTypeDto>>(packageMemberType);
                var configrationMembertypes = await unitOfWork.Repository<MemberType>().GetAllAsync();

                var packageRequestAuthorizationRequestConfigurationIds = new List<int> { 4, 5 };

                foreach (var packageRequest in packageRequestAuthorizationDto)
                {
                    if(packageRequest.RequestType is null || packageRequest.RequestType.Configuration is null)
                    {
                        Log.Warning($"PackageRequest with ID {packageRequest} has a null RequestType or Configuration. Skipping dropdown population for this request.");
                        continue;
                    }
                    if (packageRequestAuthorizationRequestConfigurationIds.Contains(packageRequest.RequestType.Configuration.Id))
                    {
                        foreach (var section in packageRequest.RequestType.Configuration.PackageConfigurationSections)
                        {
                            foreach (var field in section.ConfigurationFields)
                            {
                                foreach (var configrationMembertype in configrationMembertypes)
                                {
                                    if (configrationMembertype.Name == field.Name && field.FieldType.Name == "dropdown")
                                    {
                                        // Fetch the forms associated with the matching MemberType and RequestType
                                        var memberForms = await formRepository.GetFormsByTypeAndMemberTypeId(configrationMembertype.Id, packageRequest.RequestType.Id);

                                        // Initialize the list of dropdown values if it's not already initialized
                                        if (field.configDropDownValuesDtos == null)
                                        {
                                            field.configDropDownValuesDtos = new List<ConfigDropDownValuesDto>();
                                        }

                                        // Convert each form into a ConfigDropDownValuesDto and add it to the field's dropdown values
                                        foreach (var form in memberForms)
                                        {
                                            var configDropDownValuesDto = new ConfigDropDownValuesDto
                                            {
                                                Name = form.Title,
                                                Value = form.Id
                                            };
                                            field.configDropDownValuesDtos.Add(configDropDownValuesDto);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var PackageMmbertypeFormsAndConfigrationsDto = new PackageMmbertypeFormsAndConfigrations
                {
                    packageRequestAuthorizationDto = packageRequestAuthorizationDto,
                    packageMemberTypeDto = packageMemberTypeDto
                };

                return new ResponseHandler().Success(PackageMmbertypeFormsAndConfigrationsDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PackageMmbertypeFormsAndConfigrations>(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task<Response<PackageRequestAuthorizationDto>> UpdatePackageRequestAuthorization(PackageRequestAuthorizationDto packageRequestAuthorizationDto)
        {
            try
            {
                var packageRequestAuthorization = await unitOfWork.Repository<PackageRequestAuthourizations>().GetByIdAsync(packageRequestAuthorizationDto.PackageId);
                if (packageRequestAuthorization == null)
                {
                    return new ResponseHandler().NotFound<PackageRequestAuthorizationDto>($"PackageRequestAuthorization with ID {packageRequestAuthorizationDto.PackageId} not found.");
                }

                mapper.Map(packageRequestAuthorizationDto, packageRequestAuthorization);
                unitOfWork.Repository<PackageRequestAuthourizations>().Update(packageRequestAuthorization);
                await unitOfWork.Save();
                return new ResponseHandler().Success(packageRequestAuthorizationDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PackageRequestAuthorizationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> DeletePackageRequestAuthorization(int id)
        {
            try
            {
                var packageRequestAuthorization = await unitOfWork.Repository<PackageRequestAuthourizations>().GetByIdAsync(id);
                if (packageRequestAuthorization == null)
                {
                    return new ResponseHandler().NotFound<bool>($"PackageRequestAuthorization with ID {id} not found.");
                }

                unitOfWork.Repository<PackageRequestAuthourizations>().Delete(packageRequestAuthorization);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<PackageRequestAuthorizationDto>>> AddPackageRequestAuthorizations(List<PackageRequestAuthorizationDto> packageRequestAuthorizationDtos)
        {
            try
            {
                var packageRequestAuthorizations = mapper.Map<List<PackageRequestAuthourizations>>(packageRequestAuthorizationDtos);

                foreach (var authorization in packageRequestAuthorizations)
                {
                    await unitOfWork.Repository<PackageRequestAuthourizations>().Add(authorization);
                }

                await unitOfWork.Save();

                var resultDtos = mapper.Map<List<PackageRequestAuthorizationDto>>(packageRequestAuthorizations);
                return new ResponseHandler().Success(resultDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<PackageRequestAuthorizationDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<int>>> GetPackageMemberTypeIdsAsync(int packageId)
        {
            try
            {
                var packageMemberTypeIds = await repository.GetPackageMemberTypeIdsAsync(packageId);
                if (packageMemberTypeIds == null)
                {
                    return new ResponseHandler().BadRequest<List<int>>("No Members Found");

                }
                return new ResponseHandler().Success(packageMemberTypeIds);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<int>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<PackageRequestAuthorizationDto>>>
            GetPackageRequestAuthorizationsAsync(int packageId)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                // 1. Get package (with TenantId)
                var package = await unitOfWork.Repository<Package>().GetByIdAsync(packageId);

                if (package == null)
                    return new ResponseHandler()
                        .BadRequest<List<PackageRequestAuthorizationDto>>("Package not found");

                // 2. Check MAIN membership (NO include needed here)
                var hasMainMembership = await unitOfWork.Repository<Data.Membership.Entities.Membership>()
                    .GetAllWithSpecifications
                    (new GeneralSpecifications<Data.Membership.Entities.Membership>(x => x.UserId == userId &&
                    x.TenantId == package.TenantId && x.MemberTypeId == 5)
                    );

                // 3. Get package request authorizations WITH include
                var spec = new GeneralSpecifications<PackageRequestAuthourizations>(
                    x => x.PackageId == packageId &&
                         x.Package.TenantId == package.TenantId
                );

                spec.AddInclude(q => q.Include(x => x.Package));
                spec.AddInclude(q => q.Include(x => x.RequestType));


                var packageRequestAuthorizations = await unitOfWork
                    .Repository<PackageRequestAuthourizations>()
                    .GetAllWithSpecifications(spec);

                // 4. Apply business rules based on the member's membership status
                //    - Non-members can only start a New Membership Request (6-21)
                //    - Main members can do everything EXCEPT re-applying for a New
                //      Membership (6); ParentApplication (21) requires a parent membership
                var memberRequestIds = new[] { 7, 8, 9, 10, 12, 15, 16, 17, 18, 19, 20 };
                var nonMemberRequestIds = new[] { 6 ,21};

                var filtered = hasMainMembership.Any()
                    ? packageRequestAuthorizations
                        .Where(x => memberRequestIds.Contains(x.RequestAuthourizationId))
                    : packageRequestAuthorizations
                        .Where(x => nonMemberRequestIds.Contains(x.RequestAuthourizationId));

                // 5. Map
                var dto = mapper.Map<List<PackageRequestAuthorizationDto>>(filtered);

                return new ResponseHandler().Success(dto);
            }
            catch
            {
                return new ResponseHandler()
                    .BadRequest<List<PackageRequestAuthorizationDto>>("Something went wrong");
            }
        }
    }
}
