using AutoMapper;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.BranchLocationRepo;
using MyEntityMembershipService.Repositories.Entity.BranchMediaRepo;
using MyEntityMembershipService.Repositories.Entity.BranchOperatingHoursRepo;
using MyEntityMembershipService.Repositories.Entity.BranchRepo;
using MyEntityMembershipService.Repositories.Entity.BranchServicesRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Entity.EntityAdminService;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Entity.BranchService
{
    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchMediaRepository _branchMediaRepository;
        private readonly IBranchLocationRepository _branchLocationRepository;
        private readonly IBranchOperatingHoursRepository _branchOperatingHoursRepository;
        private readonly IBranchServicesRepository _branchServicesRepository;
        private IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEntityAdminService _entityAdminService;


        public BranchService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory, IBranchServicesRepository branchServicesRepository, IBranchOperatingHoursRepository branchOperatingHoursRepository, IBranchLocationRepository branchLocationRepository, IBranchMediaRepository branchMediaRepository, IBranchRepository branchRepository, IEntityAdminService entityAdminService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _branchRepository = branchRepository;
            _branchMediaRepository = branchMediaRepository;
            _branchLocationRepository = branchLocationRepository;
            _branchOperatingHoursRepository = branchOperatingHoursRepository;
            _branchServicesRepository = branchServicesRepository;
            this.configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _entityAdminService = entityAdminService;
        }

        public async Task<Response<BranchDto>> GetBranchByIdAsync(int branchId)
        {
            try
            {
                var branch = await _branchRepository.GetBranchByIdAsync(branchId);

                if (branch != null)
                {
                    var mappedBranch = _mapper.Map<BranchDto>(branch);

                    mappedBranch.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(mappedBranch.BranchImageURL);

                    var facTaks = mappedBranch.Facilities.Select(async facility =>
                     {
                         facility.Image = await DocumentSetting.GetPreSignedUrlAsync(facility.Image);
                         if (facility.Venues != null)
                         {
                             var venueTasks = facility.Venues.Select(async venue =>
                             {
                                 venue.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(venue.VenueImageURL);
                             });
                             await Task.WhenAll(venueTasks);
                         }
                     }).ToList();

                    await Task.WhenAll(facTaks);
                    return new ResponseHandler().Success(mappedBranch);
                }
                else
                {
                    return new ResponseHandler().NotFound<BranchDto>("Branch not found");
                }
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<BranchDto>("Something went wrong");
            }
        }
        public async Task<Response<List<BranchDto>>> GetAllBranchesAsync()
        {
            try
            {
                var branches = await _branchRepository.GetAllBranchsAsync();
                var branchDtos = _mapper.Map<List<BranchDto>>(branches);

                var tasks = branchDtos
    .Select(async branchDto =>
    {
        branchDto.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDto.BranchImageURL);

        if (branchDto.Facilities == null) return;

        var facTasks = branchDto.Facilities.Select(async facility =>
        {
            facility.Image = await DocumentSetting.GetPreSignedUrlAsync(facility.Image);

            if (facility.Venues == null) return;

            var venueTasks = facility.Venues
                .Select(async venue =>
                {
                    venue.VenueImageURL =
                        await DocumentSetting.GetPreSignedUrlAsync(venue.VenueImageURL);
                })
                .ToList();

            await Task.WhenAll(venueTasks);
        })
        .ToList();

        await Task.WhenAll(facTasks);
    })
    .ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(branchDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<BranchDto>>("Something went wrong");
            }
        }
        public async Task<Response<AddBranchDto>> AddBranchAsync(AddBranchDto branchDto)
        {
            try
            {
                var branch = _mapper.Map<Branch>(branchDto);

                if (branchDto.BranchImg != null && branchDto.BranchImg.Length > 0)
                {
                    var branchImageUrl = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", branchDto.BranchImg);
                    branch.BranchImageURL = branchImageUrl;
                }

                await _unitOfWork.Repository<Branch>().Add(branch);
                await _unitOfWork.Save();

                // if (branchDto.Branchlocation != null)
                // {
                //     var location = _mapper.Map<BranchLocation>(branchDto.Branchlocation);
                //     location.BranchID = branch.Id;
                //     await _unitOfWork.Repository<BranchLocation>().Add(location);
                // }

                if (branchDto.BranchUploads != null && branchDto.BranchUploads.Count > 0)
                {
                    foreach (var file in branchDto.BranchUploads)
                    {
                        if (file.Length > 0)
                        {
                            var url = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", file);
                            await _unitOfWork.Repository<BranchImages>().Add(new BranchImages { ImageUrl = url, BranchID = branch.Id });
                        }
                    }
                }

                // if (branchDto.BranchServices != null && branchDto.BranchServices.Count > 0)
                // {
                //     foreach (var serviceId in branchDto.BranchServices)
                //     {
                //         var branchService = new BranchServices
                //         {
                //             BranchID = branch.Id,
                //             CoreServiceID = serviceId
                //         };
                //         await _unitOfWork.Repository<BranchServices>().Add(branchService);
                //     }
                // }

                await _unitOfWork.Save();
                var addedBranchDto = _mapper.Map<AddBranchDto>(branch);
                addedBranchDto.BranchID = branch.Id;
                return new ResponseHandler().Success(addedBranchDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AddBranchDto>("Something went wrong");
            }
        }
        public async Task<Response<bool>> AddBranchOperatingHoursAsync(BranchOperatingHoursDto? operatingHoursDto)
        {
            try
            {
                var operatingHours = new BranchOperatingHours
                {
                    BranchID = operatingHoursDto.BranchID,

                    SundayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.SundayOpenAt) ? TimeOnly.Parse(operatingHoursDto.SundayOpenAt) : null,
                    SundayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.SundayCloseAt) ? TimeOnly.Parse(operatingHoursDto.SundayCloseAt) : null,
                    SundayIsWorking = operatingHoursDto.SundayIsWorking,

                    MondayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.MondayOpenAt) ? TimeOnly.Parse(operatingHoursDto.MondayOpenAt) : null,
                    MondayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.MondayCloseAt) ? TimeOnly.Parse(operatingHoursDto.MondayCloseAt) : null,
                    MondayIsWorking = operatingHoursDto.MondayIsWorking,

                    TuesdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.TuesdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.TuesdayOpenAt) : null,
                    TuesdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.TuesdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.TuesdayCloseAt) : null,
                    TuesdayIsWorking = operatingHoursDto.TuesdayIsWorking,

                    WednesdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.WednesdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.WednesdayOpenAt) : null,
                    WednesdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.WednesdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.WednesdayCloseAt) : null,
                    WednesdayIsWorking = operatingHoursDto.WednesdayIsWorking,

                    ThursdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.ThursdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.ThursdayOpenAt) : null,
                    ThursdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.ThursdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.ThursdayCloseAt) : null,
                    ThursdayIsWorking = operatingHoursDto.ThursdayIsWorking,

                    FridayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.FridayOpenAt) ? TimeOnly.Parse(operatingHoursDto.FridayOpenAt) : null,
                    FridayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.FridayCloseAt) ? TimeOnly.Parse(operatingHoursDto.FridayCloseAt) : null,
                    FridayIsWorking = operatingHoursDto.FridayIsWorking,

                    SaturdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.SaturdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.SaturdayOpenAt) : null,
                    SaturdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.SaturdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.SaturdayCloseAt) : null,
                    SaturdayIsWorking = operatingHoursDto.SaturdayIsWorking
                };

                await _unitOfWork.Repository<BranchOperatingHours>().Add(operatingHours);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<bool>> RemoveBranchAsync(int branchId)
        {
            try
            {
                var branch = await _unitOfWork.Repository<Branch>().GetWithIncludesAsync(
                    branchId,
                    b => b.BranchLocation,
                    b => b.BranchOperatingHours,
                    b => b.BranchImages,
                    b => b.BranchServices,
                    b => b.NewFacilities,
                    b => b.AccessedBranches,
                    b => b.AccessedBranchesFacilities,
                    b => b.AccessedFacilitiesVenues);

                if (branch == null)
                {
                    return new ResponseHandler().NotFound<bool>("Branch not found.");
                }

                var branchCheckDto = new CreationCheckDto
                {
                    BranchId = branchId,
                    TenantId = branch.TenantId
                };

                var isLinked = await LinkageHelper.IsLinkedToProgramAsync(branchCheckDto, _httpClientFactory, _httpContextAccessor);

                if (!isLinked)
                {
                    return new ResponseHandler().BadRequest<bool>("Branch is linked to a program; deletion is not allowed.");
                }

                if (branch.BranchLocation != null)
                {
                    _unitOfWork.Repository<BranchLocation>().SoftDelete(branch.BranchLocation);
                }

                if (branch.BranchOperatingHours != null)
                {
                    _unitOfWork.Repository<BranchOperatingHours>().SoftDelete(branch.BranchOperatingHours);
                }

                if (branch.BranchImages != null && branch.BranchImages.Any())
                {
                    foreach (var branchImage in branch.BranchImages)
                    {
                        _unitOfWork.Repository<BranchImages>().SoftDelete(branchImage);
                    }
                }

                if (branch.BranchServices != null && branch.BranchServices.Any())
                {
                    foreach (var branchService in branch.BranchServices)
                    {
                        _unitOfWork.Repository<BranchServices>().SoftDelete(branchService);
                    }
                }

                if (branch.NewFacilities != null && branch.NewFacilities.Any())
                {
                    foreach (var facility in branch.NewFacilities)
                    {
                        _unitOfWork.Repository<NewFacility>().SoftDelete(facility);
                    }
                }

                if (branch.AccessedBranches != null && branch.AccessedBranches.Any())
                {
                    foreach (var accessedBranch in branch.AccessedBranches)
                    {
                        _unitOfWork.Repository<AccessedBranches>().SoftDelete(accessedBranch);
                    }
                }

                if (branch.AccessedBranchesFacilities != null && branch.AccessedBranchesFacilities.Any())
                {
                    foreach (var accessedBranchesFacility in branch.AccessedBranchesFacilities)
                    {
                        _unitOfWork.Repository<AccessedBranchesFacilities>().SoftDelete(accessedBranchesFacility);
                    }
                }

                if (branch.AccessedFacilitiesVenues != null && branch.AccessedFacilitiesVenues.Any())
                {
                    foreach (var accessedFacilityVenue in branch.AccessedFacilitiesVenues)
                    {
                        _unitOfWork.Repository<AccessedFacilitiesVenues>().SoftDelete(accessedFacilityVenue);
                    }
                }

                _unitOfWork.Repository<Branch>().SoftDelete(branch);

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true, "Branch and all related entities successfully deleted.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<BranchDto>> UpdateBranchAsync(BranchDto branchDto)
        {
            try
            {
                var existingBranch = await _branchRepository.GetBranchByIdAsync(branchDto.Id);
                if (existingBranch == null)
                {
                    return new ResponseHandler().NotFound<BranchDto>("Branch not found");
                }

                _mapper.Map(branchDto, existingBranch);

                _unitOfWork.Repository<Branch>().Update(existingBranch);
                await _unitOfWork.Save();

                var updatedBranchDto = _mapper.Map<BranchDto>(existingBranch);
                return new ResponseHandler().Success(updatedBranchDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<BranchDto>("Something went wrong");
            }
        }
        public async Task<Response<List<BranchDto>>> GetBranchByTenantIdAsync(int tenantId, string? searchTerm, int? pageIndex, int? pageSize)
        {
            try
            {
                var branches = await _branchRepository.GetBranchByTenantIdAsync(tenantId, searchTerm, pageIndex, pageSize);
                var branchDtos = _mapper.Map<List<BranchDto>>(branches);

                var branchTasks = branchDtos.Select(async branch =>
                {
                    branch.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(branch.BranchImageURL);

                    if (branch.Facilities != null)
                    {
                        var facilityTasks = branch.Facilities.Select(async facility =>
                        {
                            facility.Image = await DocumentSetting.GetPreSignedUrlAsync(facility.Image);

                            if (facility.Venues != null)
                            {
                                var venueTasks = facility.Venues.Select(async venue =>
                                {
                                    venue.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(venue.VenueImageURL);
                                });

                                await Task.WhenAll(venueTasks);
                            }
                        });

                        await Task.WhenAll(facilityTasks);
                    }
                });

                await Task.WhenAll(branchTasks);


                object Meta = null;
                if (pageIndex.HasValue && pageSize.HasValue)
                {
                    var branchesCount = await _unitOfWork.Repository<Branch>().GetCountByCriteriaAsync(b => b.TenantId == tenantId);
                    Meta = new
                    {
                        PageIndex = pageIndex.Value,
                        PageSize = pageSize.Value,
                        TotalCount = branchesCount,
                        TotalPages = (int)Math.Ceiling((double)branchesCount / pageSize.Value)
                    };
                }

                return new ResponseHandler().Success(branchDtos, Meta: Meta!);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<BranchDto>>("Something went wrong");
            }
        }

        public async Task<Response<IEnumerable<int>>> GetVenuesOfTenantByIdAsync(int tenantId)
        {
            try
            {
                var Ids = await _branchRepository.GetVenuesOfTenantByIdAsync(tenantId);

                return new ResponseHandler().Success(Ids);
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<IEnumerable<int>>("Something went wrong");
            }
        }

        public async Task<Response<BranchDetailsDto>> GetBranchDetailsByIdAsync(int branchID)
        {
            try
            {
                var branchDetails = await _branchRepository.GetBranchDetailsByIdAsync(branchID);

                if (branchDetails is null)
                {
                    return new ResponseHandler().NotFound<BranchDetailsDto>("Branch details not found");
                }

                branchDetails.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDetails.ImageURL);

                var tasks = branchDetails.BranchImages.Select(async image =>
                {
                    image.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(image.ImageUrl);
                }).ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(branchDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<BranchDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<BranchListDetailsDto>> GetBranchCardList(int pageNumber, int tenantID)
        {
            try
            {
                var cardDetails = await _branchRepository.GetBranchCardsAsync(pageNumber, tenantID);

                if (cardDetails is null)
                {
                    return new ResponseHandler().NotFound<BranchListDetailsDto>("Cards details not found");
                }

                var tasks = cardDetails.Branches.Select(async branch =>
                {
                    branch.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(branch.ImageURL);
                }).ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(cardDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<BranchListDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<bool>> RemoveBranchMediaAsync(int branchId, int mediaId)
        {
            try
            {
                var branch = await _unitOfWork.Repository<Branch>().GetWithIncludesAsync(
                    branchId,
                    b => b.BranchImages);

                if (branch == null)
                {
                    return new ResponseHandler().NotFound<bool>("Branch not found.");
                }

                var branchImage = branch.BranchImages.FirstOrDefault(bi => bi.Id == mediaId);

                if (branchImage == null)
                {
                    return new ResponseHandler().NotFound<bool>("Media not found.");
                }

                _unitOfWork.Repository<BranchImages>().SoftDelete(branchImage);

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true, "Media successfully deleted from the branch.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<EditBranchDto>> EditBranchAsync(EditBranchDto branchDto)
        {
            try
            {
                var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(branchDto.BranchID);
                if (branch == null)
                {
                    return new ResponseHandler().NotFound<EditBranchDto>("Branch not found");
                }

                if (!string.IsNullOrEmpty(branchDto.Name))
                    branch.Name = branchDto.Name;

                if (!string.IsNullOrEmpty(branchDto.Location))
                    branch.Location = branchDto.Location;

                if (branchDto.CountryID != 0)
                    branch.CountryID = branchDto.CountryID;

                if (branchDto.StateID != 0)
                    branch.StateID = branchDto.StateID;

                if (branchDto.CityID != 0)
                    branch.CityID = branchDto.CityID;

                if (branchDto.PostalCode != 0)
                    branch.PostalCode = branchDto.PostalCode;

                if (!string.IsNullOrEmpty(branchDto.BranchManagerID))
                    branch.BranchManagerID = branchDto.BranchManagerID;

                if (!string.IsNullOrEmpty(branchDto.Description))
                    branch.Description = branchDto.Description;

                if (!string.IsNullOrEmpty(branchDto.CountryCodeID))
                    branch.CountryCodeID = branchDto.CountryCodeID;

                if (branchDto.PhoneNumber != 0)
                    branch.PhoneNumber = branchDto.PhoneNumber;

                if (!string.IsNullOrEmpty(branchDto.EmailAddress))
                    branch.EmailAddress = branchDto.EmailAddress;

                if (branchDto.BranchImg is not null && branchDto.BranchImg.Length > 0)
                {
                    var branchImageUrl = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", branchDto.BranchImg);
                    branch.BranchImageURL = branchImageUrl;
                }

                // Update branch location if provided
                if (branchDto.BranchLocation is not null)
                {
                    var branchLocation = await _branchLocationRepository.GetBranchLocationhByBranchIdAsync(branchDto.BranchID);

                    if (branchLocation is not null)
                    {
                        branchLocation.Latitude = branchDto.BranchLocation.Latitude;
                        branchLocation.Longitude = branchDto.BranchLocation.Longitude;
                        branchLocation.Name = branchDto.BranchLocation.Name;

                        _unitOfWork.Repository<BranchLocation>().Update(branchLocation);
                    }
                }

                if (branchDto.BranchUploads is not null && branchDto.BranchUploads.Count > 0)
                {
                    foreach (var file in branchDto.BranchUploads)
                    {
                        if (file.Length > 0)
                        {
                            var url = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", file);
                            await _unitOfWork.Repository<BranchImages>().Add(new BranchImages { ImageUrl = url, BranchID = branch.Id });
                        }
                    }
                }

                //TODO: Finalize branch Services edit logic.

                if (branchDto.BranchServices is not null && branchDto.BranchServices.Count > 0)
                {
                    var branchServices = await _branchServicesRepository.GetBranchServicesByBranchIdAsync(branchDto.BranchID);

                    foreach (var service in branchServices)
                    {
                        _unitOfWork.Repository<BranchServices>().SoftDelete(service);
                    }

                    if (branch.BranchServices == null)
                    {
                        branch.BranchServices = new List<BranchServices>();
                    }

                    foreach (var serviceId in branchDto.BranchServices)
                    {
                        branch.BranchServices.Add(new BranchServices { CoreServiceID = serviceId, BranchID = branch.Id });
                    }
                }



                if (branchDto.BranchOperatingHours is not null)
                {
                    var operatingHours = await _branchOperatingHoursRepository.GetBranchOperatingHoursByBranchIdAsync(branchDto.BranchID);

                    if (operatingHours is not null)
                    {
                        operatingHours.SundayOpenAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.SundayOpenAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.SundayOpenAt) : null;
                        operatingHours.SundayCloseAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.SundayCloseAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.SundayCloseAt) : null;
                        operatingHours.SundayIsWorking = branchDto.BranchOperatingHours.SundayIsWorking;

                        operatingHours.MondayOpenAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.MondayOpenAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.MondayOpenAt) : null;
                        operatingHours.MondayCloseAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.MondayCloseAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.MondayCloseAt) : null;
                        operatingHours.MondayIsWorking = branchDto.BranchOperatingHours.MondayIsWorking;

                        operatingHours.TuesdayOpenAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.TuesdayOpenAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.TuesdayOpenAt) : null;
                        operatingHours.TuesdayCloseAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.TuesdayCloseAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.TuesdayCloseAt) : null;
                        operatingHours.TuesdayIsWorking = branchDto.BranchOperatingHours.TuesdayIsWorking;

                        operatingHours.WednesdayOpenAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.WednesdayOpenAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.WednesdayOpenAt) : null;
                        operatingHours.WednesdayCloseAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.WednesdayCloseAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.WednesdayCloseAt) : null;
                        operatingHours.WednesdayIsWorking = branchDto.BranchOperatingHours.WednesdayIsWorking;

                        operatingHours.ThursdayOpenAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.ThursdayOpenAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.ThursdayOpenAt) : null;
                        operatingHours.ThursdayCloseAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.ThursdayCloseAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.ThursdayCloseAt) : null;
                        operatingHours.ThursdayIsWorking = branchDto.BranchOperatingHours.ThursdayIsWorking;

                        operatingHours.FridayOpenAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.FridayOpenAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.FridayOpenAt) : null;
                        operatingHours.FridayCloseAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.FridayCloseAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.FridayCloseAt) : null;
                        operatingHours.FridayIsWorking = branchDto.BranchOperatingHours.FridayIsWorking;

                        operatingHours.SaturdayOpenAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.SaturdayOpenAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.SaturdayOpenAt) : null;
                        operatingHours.SaturdayCloseAt = !string.IsNullOrEmpty(branchDto.BranchOperatingHours.SaturdayCloseAt) ? TimeOnly.Parse(branchDto.BranchOperatingHours.SaturdayCloseAt) : null;
                        operatingHours.SaturdayIsWorking = branchDto.BranchOperatingHours.SaturdayIsWorking;

                        _unitOfWork.Repository<BranchOperatingHours>().Update(operatingHours);
                    }
                }


                _unitOfWork.Repository<Branch>().Update(branch);
                await _unitOfWork.Save();

                var updatedBranchDto = new EditBranchDto
                {
                    BranchID = branch.Id,
                    Name = branch.Name,
                    CountryID = branch.CountryID,
                    StateID = branch.StateID,
                    CityID = branch.CityID,
                    PostalCode = branch.PostalCode,
                    BranchManagerID = branch.BranchManagerID,
                    Description = branch.Description,
                    CountryCodeID = branch.CountryCodeID,
                    PhoneNumber = branch.PhoneNumber,
                    EmailAddress = branch.EmailAddress,
                    BranchLocation = branch.BranchLocation != null ? new BranchLocationDto
                    {
                        Latitude = branch.BranchLocation.Latitude,
                        Longitude = branch.BranchLocation.Longitude,
                        Name = branch.BranchLocation.Name
                    } : null,
                    BranchServices = branch.BranchServices.Select(x => x.CoreServiceID).ToList()
                };

                return new ResponseHandler().Success(updatedBranchDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EditBranchDto>("Something went wrong");
            }
        }
        public async Task<Response<EditBranchDetailsDto>> GetBranchEditDetailsByIdAsync(int branchID)
        {
            try
            {
                var branchDetails = await _branchRepository.GetBranchEditDetailsByIdAsync(branchID);

                if (branchDetails is null)
                {
                    return new ResponseHandler().NotFound<EditBranchDetailsDto>("Branch details not found");
                }

                branchDetails.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDetails.BranchImageURL);


                return new ResponseHandler().Success(branchDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EditBranchDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<BranchMobileCardListDto>> GetBranchCardsByTenantID(int pageNumber, int tenantId)
        {
            try
            {
                var branchDetails = await _branchRepository.GetBranchCardsByTenantID(pageNumber, tenantId);

                if (branchDetails is null)
                {
                    return new ResponseHandler().NotFound<BranchMobileCardListDto>("Branches list for this tanent not found");
                }

                var taks = branchDetails.Branches.Select(async branch =>
                {
                    branch.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(branch.ImageURL);
                }).ToList();
                await Task.WhenAll(taks);

                return new ResponseHandler().Success(branchDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<BranchMobileCardListDto>("Something went wrong");
            }
        }
        public async Task<Response<BranchMobileDetailsDto>> GetBranchMobileDetailsByID(int branchID)
        {
            try
            {
                var branchDetails = await _branchRepository.GetBranchDetailsMobileView(branchID);

                if (branchDetails is null)
                {
                    return new ResponseHandler().NotFound<BranchMobileDetailsDto>("Branches details not found");
                }

                branchDetails.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDetails.ImageURL);

                var tasks = branchDetails.BranchMedia.Select(async media =>
                {
                    media.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(media.ImageURL);
                }).ToList();

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(branchDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<BranchMobileDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<List<MobileBranchFacilitiesDto>>> GetMobileBranchFacilities(int branchID)
        {
            try
            {
                var facilityList = await _branchRepository.GetMobileBranchFacilitiesAsync(branchID);

                if (facilityList is null)
                {
                    return new ResponseHandler().NotFound<List<MobileBranchFacilitiesDto>>("Branches facilities not found");
                }

                var facTasks = facilityList.Select(async facility =>
                {
                    facility.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(facility.ImageURL);
                }).ToList();

                await Task.WhenAll(facTasks);

                return new ResponseHandler().Success(facilityList);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MobileBranchFacilitiesDto>>("Something went wrong");
            }
        }
        public async Task<Response<bool>> AddBranchNewMedia(List<IFormFile> newMedia, int branchID)
        {
            try
            {
                var branch = await _branchRepository.GetBranchByIdAsync(branchID);

                if (branch is null)
                {
                    return new ResponseHandler().NotFound<bool>("Branch not found");
                }

                if (newMedia is not null && newMedia.Count > 0)
                {
                    foreach (var file in newMedia)
                    {
                        if (file.Length > 0)
                        {
                            var url = await DocumentSetting.AddToContainerAndGetUrl(
                                configuration["StorageConnectionString"],
                                "buildupimagedocs",
                                file
                            );

                            await _unitOfWork.Repository<BranchImages>().Add(
                                new BranchImages
                                {
                                    ImageUrl = url,
                                    BranchID = branchID
                                }
                            );
                        }
                    }

                    await _unitOfWork.Save();
                }

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<bool>> UpdateBranchOperatingHoursAsync(EditBranchOperatingHoursDto branchDto)
        {
            var branchOperatingHours = await _branchOperatingHoursRepository.GetBranchOperatingHoursByBranchIdAsync(branchDto.BranchID);

            TimeOnly? ParseTime(string time)
            {
                return TimeOnly.TryParse(time, out var parsedTime) ? parsedTime : null;
            }

            Response<bool>? ValidateDayTimes(TimeOnly? openAt, TimeOnly? closeAt, string dayName)
            {
                if (openAt.HasValue && closeAt.HasValue && openAt >= closeAt)
                    return new ResponseHandler().BadRequest<bool>($"{dayName} open time must be earlier than close time.");

                return null;
            }

            // Parse all times
            var sundayOpenAt = ParseTime(branchDto.SundayOpenAt);
            var sundayCloseAt = ParseTime(branchDto.SundayCloseAt);
            var mondayOpenAt = ParseTime(branchDto.MondayOpenAt);
            var mondayCloseAt = ParseTime(branchDto.MondayCloseAt);
            var tuesdayOpenAt = ParseTime(branchDto.TuesdayOpenAt);
            var tuesdayCloseAt = ParseTime(branchDto.TuesdayCloseAt);
            var wednesdayOpenAt = ParseTime(branchDto.WednesdayOpenAt);
            var wednesdayCloseAt = ParseTime(branchDto.WednesdayCloseAt);
            var thursdayOpenAt = ParseTime(branchDto.ThursdayOpenAt);
            var thursdayCloseAt = ParseTime(branchDto.ThursdayCloseAt);
            var fridayOpenAt = ParseTime(branchDto.FridayOpenAt);
            var fridayCloseAt = ParseTime(branchDto.FridayCloseAt);
            var saturdayOpenAt = ParseTime(branchDto.SaturdayOpenAt);
            var saturdayCloseAt = ParseTime(branchDto.SaturdayCloseAt);

            // Validate all days
            var validationResult = ValidateDayTimes(sundayOpenAt, sundayCloseAt, "Sunday")
                                ?? ValidateDayTimes(mondayOpenAt, mondayCloseAt, "Monday")
                                ?? ValidateDayTimes(tuesdayOpenAt, tuesdayCloseAt, "Tuesday")
                                ?? ValidateDayTimes(wednesdayOpenAt, wednesdayCloseAt, "Wednesday")
                                ?? ValidateDayTimes(thursdayOpenAt, thursdayCloseAt, "Thursday")
                                ?? ValidateDayTimes(fridayOpenAt, fridayCloseAt, "Friday")
                                ?? ValidateDayTimes(saturdayOpenAt, saturdayCloseAt, "Saturday");

            if (validationResult != null)
                return validationResult;

            if (branchOperatingHours == null)
            {
                // ADD
                branchOperatingHours = new BranchOperatingHours
                {
                    BranchID = branchDto.BranchID,

                    SundayOpenAt = sundayOpenAt,
                    SundayCloseAt = sundayCloseAt,
                    SundayIsWorking = branchDto.SundayIsWorking,

                    MondayOpenAt = mondayOpenAt,
                    MondayCloseAt = mondayCloseAt,
                    MondayIsWorking = branchDto.MondayIsWorking,

                    TuesdayOpenAt = tuesdayOpenAt,
                    TuesdayCloseAt = tuesdayCloseAt,
                    TuesdayIsWorking = branchDto.TuesdayIsWorking,

                    WednesdayOpenAt = wednesdayOpenAt,
                    WednesdayCloseAt = wednesdayCloseAt,
                    WednesdayIsWorking = branchDto.WednesdayIsWorking,

                    ThursdayOpenAt = thursdayOpenAt,
                    ThursdayCloseAt = thursdayCloseAt,
                    ThursdayIsWorking = branchDto.ThursdayIsWorking,

                    FridayOpenAt = fridayOpenAt,
                    FridayCloseAt = fridayCloseAt,
                    FridayIsWorking = branchDto.FridayIsWorking,

                    SaturdayOpenAt = saturdayOpenAt,
                    SaturdayCloseAt = saturdayCloseAt,
                    SaturdayIsWorking = branchDto.SaturdayIsWorking,
                };

                await _branchOperatingHoursRepository.Add(branchOperatingHours);
            }
            else
            {
                // UPDATE
                branchOperatingHours.SundayOpenAt = sundayOpenAt;
                branchOperatingHours.SundayCloseAt = sundayCloseAt;
                branchOperatingHours.SundayIsWorking = branchDto.SundayIsWorking;

                branchOperatingHours.MondayOpenAt = mondayOpenAt;
                branchOperatingHours.MondayCloseAt = mondayCloseAt;
                branchOperatingHours.MondayIsWorking = branchDto.MondayIsWorking;

                branchOperatingHours.TuesdayOpenAt = tuesdayOpenAt;
                branchOperatingHours.TuesdayCloseAt = tuesdayCloseAt;
                branchOperatingHours.TuesdayIsWorking = branchDto.TuesdayIsWorking;

                branchOperatingHours.WednesdayOpenAt = wednesdayOpenAt;
                branchOperatingHours.WednesdayCloseAt = wednesdayCloseAt;
                branchOperatingHours.WednesdayIsWorking = branchDto.WednesdayIsWorking;

                branchOperatingHours.ThursdayOpenAt = thursdayOpenAt;
                branchOperatingHours.ThursdayCloseAt = thursdayCloseAt;
                branchOperatingHours.ThursdayIsWorking = branchDto.ThursdayIsWorking;

                branchOperatingHours.FridayOpenAt = fridayOpenAt;
                branchOperatingHours.FridayCloseAt = fridayCloseAt;
                branchOperatingHours.FridayIsWorking = branchDto.FridayIsWorking;

                branchOperatingHours.SaturdayOpenAt = saturdayOpenAt;
                branchOperatingHours.SaturdayCloseAt = saturdayCloseAt;
                branchOperatingHours.SaturdayIsWorking = branchDto.SaturdayIsWorking;
            }

            await _unitOfWork.Save();

            return new ResponseHandler().Success(true);
        }

        public async Task<Response<List<BranchDto>>> GetAllBranchesByTenantIdAsync(int? tenantId, string userId)
        {
            try
            {
                if (!tenantId.HasValue)
                {
                    Log.Error("TenantId is null");
                    return new ResponseHandler().BadRequest<List<BranchDto>>("TenantId is null");
                }

                var tenantAdmin = await _entityAdminService.GetTenantAdminByUserId(userId);
                if (!tenantAdmin.Succeeded || tenantAdmin.Data is null)
                {
                    Log.Error("Failed to retrieve tenant admin for userId {UserId}", userId);
                }
                var tenantAdminuserId = tenantAdmin?.Data?.UserId;
                var spec = new GeneralSpecifications<Branch>(
                b => b.TenantId == tenantId.Value &&
                (
                tenantAdmin == null ||
                string.IsNullOrEmpty(tenantAdminuserId) ||
        tenantAdmin.Data.BranchIds == null ||
        !tenantAdmin.Data.BranchIds.Any() ||
        tenantAdmin.Data.BranchIds.Contains(b.Id)
    )
);


                var branches = await _unitOfWork.Repository<Branch>()
                                               .GetAllWithSpecifications(spec);
                Log.Information("Fetched {Count} accessed branches for tenantId {TenantId}", branches.Count, tenantId);

                var branchDtos = branches.Select(b => new BranchDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Location = b.Location,
                    BranchImageURL = b.BranchImageURL
                }
                ).ToList();

                var tasks = branchDtos.Select(async dto =>
                {
                    dto.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(

                        dto.BranchImageURL);
                }).ToList();

                await Task.WhenAll(tasks);

                Log.Information("Mapped accessed branches to DTOs. Total DTOs: {Count}", branchDtos.Count);

                return new ResponseHandler().Success(branchDtos);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching package branches.");
                return new ResponseHandler().BadRequest<List<BranchDto>>("Something went wrong");

            }

        }

        public async Task<Response<PaginationToReturnDto<BranchLookupDto>>> GetBranchesByTenantAsync(BranchLookupSearchDto dto)
        {
            try
            {
                if (!dto.TenantId.HasValue || dto.TenantId <= 0)
                {
                    Log.Error("Invalid TenantId: {TenantId}", dto.TenantId);
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<BranchLookupDto>>("Invalid TenantId.");
                }

                Expression<Func<Branch, bool>> criteria = b =>
                    b.TenantId == dto.TenantId &&
                    (string.IsNullOrEmpty(dto.SearchTerm) || b.Name.Trim().ToLower().Contains(dto.SearchTerm.Trim().ToLower()));

                Expression<Func<Branch, BranchLookupDto>> selector = b => new BranchLookupDto
                {
                    Id = b.Id,
                    Name = b.Name
                };


                // Count total items matching the criteria for pagination metadata
                var countSpec = new GeneralSpecifications<Branch>(criteria);
                var totalCount = await _unitOfWork.Repository<Branch>().GetCountByCriteriaAsync(criteria);


                var spec = new GeneralSpecifications<Branch>(criteria, dto.PageIndex, dto.PageSize);
                spec.SetOrderByDescending(b => b.Id);
                spec.AsNoTracking = true;

                var data = await _unitOfWork.Repository<Branch>()
                            .GetAllWithSpecificationsAndProjection(spec, selector);

                Log.Information("Fetched {Count} branches for tenantId {TenantId} with search term '{SearchTerm}'", data.Count(), dto.TenantId, dto.SearchTerm);

                int totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

                var result = new PaginationToReturnDto<BranchLookupDto>(
                    count: totalCount,
                    totalPages: totalPages,
                    pageSize: dto.PageSize,
                    pageNumber: dto.PageIndex,
                    data: data
                );

                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching branches for tenantId {TenantId}", dto.TenantId);
                return new ResponseHandler().BadRequest<PaginationToReturnDto<BranchLookupDto>>("Something went wrong.");
            }
        }
    }
}
