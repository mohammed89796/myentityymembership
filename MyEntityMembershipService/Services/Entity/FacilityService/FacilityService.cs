using AutoMapper;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.FacilityDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.FacilityOperatingHoursRepo;
using MyEntityMembershipService.Repositories.Entity.FacilityRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Entity.FacilityService
{
    public class FacilityService : IFacilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFacilityRepository _facilityRepository;
        private readonly IFacilityOperatingHoursRepository _facilityOperatingHoursRepository;
        private IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;


        public FacilityService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory, IFacilityOperatingHoursRepository facilityOperatingHoursRepository, IFacilityRepository facilityRepository, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _facilityRepository = facilityRepository;
            _facilityOperatingHoursRepository = facilityOperatingHoursRepository;
            this.configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;

        }

        public async Task<Response<AddFacilityDto>> AddFacilityAsync(AddFacilityDto facilityDto)
        {
            try
            {
                var facility = _mapper.Map<NewFacility>(facilityDto);

                // Upload the main facility image if it exists
                if (facilityDto.FacilityImage != null && facilityDto.FacilityImage.Length > 0)
                {
                    var facilityImageUrl = await DocumentSetting.AddToContainerAndGetUrl(
                        configuration["StorageConnectionString"],
                        "buildupimagedocs",
                        facilityDto.FacilityImage
                    );
                    facility.FacilityImageUrl = facilityImageUrl;
                }

                // Add the new facility
                await _unitOfWork.Repository<NewFacility>().Add(facility);
                await _unitOfWork.Save();

                // Handle facility location
                //if (facilityDto.FacilityLocation != null)
                //{
                //    // var location = _mapper.Map<FacilityLocation>(facilityDto.FacilityLocation);
                //    var facilityLocation = new FacilityLocation
                //    {
                //        Name = facilityDto.FacilityLocation.Name,
                //        Latitude = facilityDto.FacilityLocation.Latitude,
                //        Longitude = facilityDto.FacilityLocation.Longitude,
                //        FacilityID = facility.Id
                //    };
                //    await _unitOfWork.Repository<FacilityLocation>().Add(facilityLocation);
                //}

                //// Handle facility amenities
                //if (facilityDto.FacilityAmenities != null && facilityDto.FacilityAmenities.Any())
                //{
                //    foreach (var amenityId in facilityDto.FacilityAmenities)
                //    {
                //        var facilityAmenity = new NewFacilityAmenities
                //        {
                //            FacilityId = facility.Id,
                //            AmenityID = amenityId
                //        };
                //        await _unitOfWork.Repository<NewFacilityAmenities>().Add(facilityAmenity);
                //    }
                //}

                //// Handle facility services
                //if (facilityDto.FacilityServices != null && facilityDto.FacilityServices.Any())
                //{
                //    foreach (var serviceId in facilityDto.FacilityServices)
                //    {
                //        var facilityService = new FacilityServices
                //        {
                //            FacilityID = facility.Id,
                //            CoreFacilityServiceID = serviceId
                //        };
                //        await _unitOfWork.Repository<FacilityServices>().Add(facilityService);
                //    }
                //}

                //// Handle facility accessibility options
                //var validAccessibilityOptionIds = await _unitOfWork.Repository<NewAccessibilityOptions>()
                //    .GetAllAsync();

                //if (facilityDto.FacilityAccessibilityOptions != null && facilityDto.FacilityAccessibilityOptions.Any())
                //{
                //    foreach (var optionId in facilityDto.FacilityAccessibilityOptions)
                //    {
                //        // Check if the optionId exists in the valid list
                //        if (!validAccessibilityOptionIds.Any(a => a.Id == optionId))
                //        {
                //            throw new Exception($"Invalid AccessibilityOptionID: {optionId}");
                //        }

                //        var facilityAccessibility = new NewFacilityAccessibilityOptions
                //        {
                //            FacilityID = facility.Id,
                //            AccessibilityOptionID = optionId
                //        };
                //        await _unitOfWork.Repository<NewFacilityAccessibilityOptions>().Add(facilityAccessibility);
                //    }
                //}
                //// Handle facility sports
                //if (facilityDto.SportsList != null && facilityDto.SportsList.Any())
                //{
                //    foreach (var sportId in facilityDto.SportsList)
                //    {
                //        var facilitySport = new NewFacilitySports
                //        {
                //            FacilityID = facility.Id,
                //            SportID = sportId
                //        };
                //        await _unitOfWork.Repository<NewFacilitySports>().Add(facilitySport);
                //    }
                //}

                //Handle additional facility image uploads, if any
                if (facilityDto.FacilityUploads != null && facilityDto.FacilityUploads.Any())
                {
                    foreach (var file in facilityDto.FacilityUploads)
                    {
                        if (file.Length > 0)
                        {
                            var url = await DocumentSetting.AddToContainerAndGetUrl(
                                configuration["StorageConnectionString"],
                                "buildupimagedocs",
                                file
                            );
                            var facilityImage = new FacilityImages
                            {
                                ImageUrl = url,
                                FacilityID = facility.Id
                            };
                            await _unitOfWork.Repository<FacilityImages>().Add(facilityImage);
                        }
                    }
                }

                // Save all the changes
                await _unitOfWork.Save();

                // Return the created facility information
                //var addedFacilityDto = _mapper.Map<AddFacilityDto>(facility);
                facilityDto.CreatedFacilityID = facility.Id;

                return new ResponseHandler().Success(facilityDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AddFacilityDto>("Something went wrong");
            }
        }
        public async Task<Response<bool>> AddFacilityOperatingHoursAsync(FacilityOperatingHoursDto? operatingHoursDto)
        {
            try
            {
                var operatingHours = new NewFacilityOperatingHours
                {
                    FacilityId = operatingHoursDto.FacilityId,

                    sundayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.SundayOpenAt) ? TimeOnly.Parse(operatingHoursDto.SundayOpenAt) : null,
                    sundayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.SundayCloseAt) ? TimeOnly.Parse(operatingHoursDto.SundayCloseAt) : null,
                    sundayIsWorking = operatingHoursDto.SundayIsWorking,

                    mondayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.MondayOpenAt) ? TimeOnly.Parse(operatingHoursDto.MondayOpenAt) : null,
                    mondayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.MondayCloseAt) ? TimeOnly.Parse(operatingHoursDto.MondayCloseAt) : null,
                    mondayIsWorking = operatingHoursDto.MondayIsWorking,

                    tuesdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.TuesdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.TuesdayOpenAt) : null,
                    tuesdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.TuesdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.TuesdayCloseAt) : null,
                    tuesdayIsWorking = operatingHoursDto.TuesdayIsWorking,

                    wednesdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.WednesdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.WednesdayOpenAt) : null,
                    wednesdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.WednesdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.WednesdayCloseAt) : null,
                    wednesdayIsWorking = operatingHoursDto.WednesdayIsWorking,

                    thursdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.ThursdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.ThursdayOpenAt) : null,
                    thursdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.ThursdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.ThursdayCloseAt) : null,
                    thursdayIsWorking = operatingHoursDto.ThursdayIsWorking,

                    fridayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.FridayOpenAt) ? TimeOnly.Parse(operatingHoursDto.FridayOpenAt) : null,
                    fridayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.FridayCloseAt) ? TimeOnly.Parse(operatingHoursDto.FridayCloseAt) : null,
                    fridayIsWorking = operatingHoursDto.FridayIsWorking,

                    saturdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.SaturdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.SaturdayOpenAt) : null,
                    saturdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.SaturdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.SaturdayCloseAt) : null,
                    saturdayIsWorking = operatingHoursDto.SaturdayIsWorking
                };

                await _unitOfWork.Repository<NewFacilityOperatingHours>().Add(operatingHours);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<FacilityListDetailsDto>> GetFaciityCardList(int pageNumber, int branchID)
        {
            try
            {
                var facilityList = await _facilityRepository.GetFacilitiesByBranchIdAsync(pageNumber, branchID);

                if (facilityList is null)
                {
                    return new ResponseHandler().NotFound<FacilityListDetailsDto>("facilities list not found");
                }

                var tasks= facilityList.Facilities.Select(async facility =>
                {
                    facility.ImageURL=await DocumentSetting.GetPreSignedUrlAsync(facility.ImageURL);
                });
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(facilityList);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FacilityListDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<FacilityDetailsDto>> GetFacilityDetailsByIdAsync(int facilityID)
        {
            try
            {
                var branchDetails = await _facilityRepository.GetFacilityDetailsByIdAsync(facilityID);

                if (branchDetails is null)
                {
                    return new ResponseHandler().NotFound<FacilityDetailsDto>("Facility details not found");
                }
                var tasks= branchDetails.FacilityImages.Select(async image =>
                {
                    branchDetails.HeaderDetails.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDetails.HeaderDetails.BranchImageURL);
                    branchDetails.HeaderDetails.FacilityImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDetails.HeaderDetails.FacilityImageURL);
                    image.ImageUrl=await DocumentSetting.GetPreSignedUrlAsync(image.ImageUrl);
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(branchDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FacilityDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<List<FacilityDetailsDto>>> GetAllFacilities()
        {
            try
            {

                Expression<Func<NewFacility, FacilityDetailsDto>> selector =
                facility => new FacilityDetailsDto
                {
                    FacilityID = facility.Id,
                    HeaderDetails = new FacilityHeaderDetailsDto
                    {
                        FacilityName = facility.FacilityName,
                        FacilityImageURL = facility.FacilityImageUrl,
                        Description = facility.Description,
                        FacilityTypeName = facility.FacilityType.name,
                        BranchName = facility.Branch.Name,
                        BranchImageURL = facility.Branch.BranchImageURL,
                        Email = facility.Branch.EmailAddress,
                        PhoneNumber =facility.CountryCodeID + facility.PhoneNumber,
                        Country = facility.Branch.BranchLocation.Name,
                        Address = facility.Branch.Location,
                        NumberOfVenues = facility.NewVenues.Count()
                    },
                    
                    LocationDetails = new FacilityLocationDetailsDto
                    {
                        Name = facility.FacilityLocation.Name,
                        Latitude = facility.FacilityLocation.Latitude,
                        Longitude = facility.FacilityLocation.Longitude
                    },

    
                    OperatingHours = new OperatingHoursDto
    
                    {
                        SundayOpenAt = facility.FacilityOperatingHours.sundayOpenAt,
                        SundayCloseAt = facility.FacilityOperatingHours.sundayCloseAt,
                        SundayIsWorking =facility.FacilityOperatingHours.sundayIsWorking ?? false,
                        FridayCloseAt = facility.FacilityOperatingHours.fridayCloseAt,
                        FridayIsWorking = facility.FacilityOperatingHours.fridayIsWorking ?? false,
                        FridayOpenAt = facility.FacilityOperatingHours.fridayOpenAt,
                        MondayCloseAt = facility.FacilityOperatingHours.mondayCloseAt,
                        MondayIsWorking = facility.FacilityOperatingHours.mondayIsWorking ?? false,
                        MondayOpenAt = facility.FacilityOperatingHours.mondayOpenAt,
                        SaturdayCloseAt = facility.FacilityOperatingHours.saturdayCloseAt,
                        SaturdayIsWorking = facility.FacilityOperatingHours.saturdayIsWorking ?? false,
                        SaturdayOpenAt = facility.FacilityOperatingHours.saturdayOpenAt,
                        ThursdayCloseAt = facility.FacilityOperatingHours.thursdayCloseAt,
                        ThursdayIsWorking = facility.FacilityOperatingHours.thursdayIsWorking ?? false,
                        ThursdayOpenAt = facility.FacilityOperatingHours.thursdayOpenAt,
                        TuesdayCloseAt = facility.FacilityOperatingHours.tuesdayCloseAt,
                        TuesdayIsWorking = facility.FacilityOperatingHours.tuesdayIsWorking ?? false,
                        TuesdayOpenAt = facility.FacilityOperatingHours.tuesdayOpenAt,
                        WednesdayCloseAt = facility.FacilityOperatingHours.wednesdayCloseAt,
                        WednesdayIsWorking = facility.FacilityOperatingHours.wednesdayIsWorking ?? false,
                        WednesdayOpenAt = facility.FacilityOperatingHours.wednesdayOpenAt
                    },

                    FeaturesDetails = new FacilityFeaturesDto
                    {
                    
                        FacilityAmenities = facility.FacilityAmenities.Select(x => x.Amenity.name).ToList(),
                        FacilityAccessibility =facility.FacilityAccessibilityOptions
                        .Select(x => x.AccessibilityOptions.OptionName)
                        .ToList(),
                        FacilitySports = facility.SportsList
                        .Select(x => x.Sports.SportName)
                        .ToList()
                    },

                    FacilityImages =
                    facility.FacilityUploads
                    .Select(x => new FacilityImages
                        {
                            Id = x.Id,
                            ImageUrl = x.ImageUrl,
                            FacilityID = x.FacilityID
                    })
                    .ToList()

                };


                var facilities = await _unitOfWork.Repository<NewFacility>().GetAllWithSpecificationsAndProjection(new GeneralSpecifications<NewFacility>(),selector);
                if (facilities == null || !facilities.Any())
                {
                    return new ResponseHandler().NotFound<List<FacilityDetailsDto>>("there is now facilities yet");
                }


                if (facilities is null)
                {
                    return new ResponseHandler().NotFound<List<FacilityDetailsDto>>("there is now facilities yet");
                }

                var tasks = facilities.Select(async facilityDto =>
                {
                    if (facilityDto.FacilityImages == null || !facilityDto.FacilityImages.Any())
                        return;

                    var facTasks = facilityDto.FacilityImages.Select(async image =>
                    {
                        image.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(image.ImageUrl);
                    });

                    await Task.WhenAll(facTasks);
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(facilities);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<FacilityDetailsDto>>("Something went wrong");
            }
        }
        public async Task<Response<bool>> RemoveFacilityAsync(int facilityId)
        {
            try
            {
                var facility = await _unitOfWork.Repository<NewFacility>().GetWithIncludesAsync(
                    facilityId,
                    f => f.FacilityLocation,
                    f => f.FacilityOperatingHours,
                    f => f.FacilityAmenities,
                    f => f.FacilityServices,
                    f => f.FacilityAccessibilityOptions,
                    f => f.SportsList,
                    f => f.FacilityUploads,
                    f => f.NewVenues);

                if (facility == null)
                {
                    return new ResponseHandler().NotFound<bool>("Facility not found.");
                }

                var facilityCheckDto = new CreationCheckDto
                {
                    FacilityId = facilityId,
                    TenantId = facility.Branch.TenantId
                };

                var isLinked = await LinkageHelper.IsLinkedToProgramAsync(facilityCheckDto, _httpClientFactory, _httpContextAccessor);

                if (!isLinked)
                {
                    return new ResponseHandler().BadRequest<bool>("Branch is linked to a program; deletion is not allowed.");
                }

                if (facility.FacilityLocation != null)
                {
                    _unitOfWork.Repository<FacilityLocation>().SoftDelete(facility.FacilityLocation);
                }

                if (facility.FacilityOperatingHours != null)
                {
                    _unitOfWork.Repository<NewFacilityOperatingHours>().SoftDelete(facility.FacilityOperatingHours);
                }

                if (facility.FacilityAmenities != null && facility.FacilityAmenities.Any())
                {
                    foreach (var amenity in facility.FacilityAmenities)
                    {
                        _unitOfWork.Repository<NewFacilityAmenities>().SoftDelete(amenity);
                    }
                }

                if (facility.FacilityServices != null && facility.FacilityServices.Any())
                {
                    foreach (var service in facility.FacilityServices)
                    {
                        _unitOfWork.Repository<FacilityServices>().SoftDelete(service);
                    }
                }

                if (facility.FacilityAccessibilityOptions != null && facility.FacilityAccessibilityOptions.Any())
                {
                    foreach (var accessibilityOption in facility.FacilityAccessibilityOptions)
                    {
                        _unitOfWork.Repository<NewFacilityAccessibilityOptions>().SoftDelete(accessibilityOption);
                    }
                }

                if (facility.SportsList != null && facility.SportsList.Any())
                {
                    foreach (var sport in facility.SportsList)
                    {
                        _unitOfWork.Repository<NewFacilitySports>().SoftDelete(sport);
                    }
                }

                if (facility.FacilityUploads != null && facility.FacilityUploads.Any())
                {
                    foreach (var upload in facility.FacilityUploads)
                    {
                        _unitOfWork.Repository<FacilityImages>().SoftDelete(upload);
                    }
                }

                if (facility.NewVenues != null && facility.NewVenues.Any())
                {
                    foreach (var venue in facility.NewVenues)
                    {
                        _unitOfWork.Repository<NewVenue>().SoftDelete(venue);
                    }
                }

                _unitOfWork.Repository<NewFacility>().SoftDelete(facility);

                // Save changes
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true, "Facility and all related entities successfully deleted.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<bool>> RemoveFacilityMediaAsync(int facilityID, int mediaId)
        {
            try
            {
                var facility = await _unitOfWork.Repository<NewFacility>().GetWithIncludesAsync(
                    facilityID,
                    b => b.FacilityUploads);

                if (facility == null)
                {
                    return new ResponseHandler().NotFound<bool>("Facility not found.");
                }


                var facilityImage = facility.FacilityUploads.FirstOrDefault(bi => bi.Id == mediaId);

                if (facilityImage == null)
                {
                    return new ResponseHandler().NotFound<bool>("Media not found.");
                }

                _unitOfWork.Repository<FacilityImages>().SoftDelete(facilityImage);

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true, "Media successfully deleted from the facility.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<EditFacilityDto>> EditFacilityAsync(EditFacilityDto facilityDto)
        {
            try
            {
                var facility = await _unitOfWork.Repository<NewFacility>().GetWithIncludesAsync(
                    facilityDto.FacilityId,
                    f => f.FacilityLocation,
                    f => f.FacilityOperatingHours,
                    f => f.FacilityAmenities,
                    f => f.FacilityServices,
                    f => f.FacilityAccessibilityOptions,
                    f => f.SportsList);

                if (facility == null)
                {
                    return new ResponseHandler().NotFound<EditFacilityDto>("Facility not found");
                }

                if (facilityDto.FacilityImageUrl is not null && facilityDto.FacilityImageUrl.Length > 0)
                {
                    var facilityImageUrl = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", facilityDto.FacilityImageUrl);
                    facility.FacilityImageUrl = facilityImageUrl;
                }

                if (!string.IsNullOrEmpty(facilityDto.FacilityName))
                    facility.FacilityName = facilityDto.FacilityName;

                if (facilityDto.FacilityTypeId != 0)
                    facility.FacilityTypeId = facilityDto.FacilityTypeId;

                if (facilityDto.FacilityPremisesTypeID != 0)
                    facility.FacilityPremisesTypeID = facilityDto.FacilityPremisesTypeID;

                if (!string.IsNullOrEmpty(facilityDto.Description))
                    facility.Description = facilityDto.Description;

                if (facilityDto.FacilityUploads is not null && facilityDto.FacilityUploads.Count > 0)
                {
                    foreach (var file in facilityDto.FacilityUploads)
                    {
                        if (file.Length > 0)
                        {
                            var url = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", file);
                            facility.FacilityImageUrl = url;
                        }
                    }
                }

                if (facilityDto.FacilityLocation is not null)
                {
                    var facilityLocation = await _unitOfWork.Repository<FacilityLocation>().GetByIdAsync(facility.FacilityLocation.Id);
                    if (facilityLocation is not null)
                    {
                        facilityLocation.Latitude = facilityDto.FacilityLocation.Latitude;
                        facilityLocation.Longitude = facilityDto.FacilityLocation.Longitude;
                        facilityLocation.Name = facilityDto.FacilityLocation.Name;
                        _unitOfWork.Repository<FacilityLocation>().Update(facilityLocation);
                    }
                }

                if (facilityDto.FacilityOperatingHours is not null)
                {
                    var operatingHours = await _facilityOperatingHoursRepository.GetFacilityOperatingHoursByFacilityIdAsync(facilityDto.FacilityId);
                    if (operatingHours is not null)
                    {
                        // Sunday
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.SundayOpenAt))
                        {
                            operatingHours.sundayOpenAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.SundayOpenAt);
                        }
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.SundayCloseAt))
                        {
                            operatingHours.sundayCloseAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.SundayCloseAt);
                        }
                        operatingHours.sundayIsWorking = facilityDto.FacilityOperatingHours.SundayIsWorking ?? operatingHours.sundayIsWorking;

                        // Monday
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.MondayOpenAt))
                        {
                            operatingHours.mondayOpenAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.MondayOpenAt);
                        }
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.MondayCloseAt))
                        {
                            operatingHours.mondayCloseAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.MondayCloseAt);
                        }
                        operatingHours.mondayIsWorking = facilityDto.FacilityOperatingHours.MondayIsWorking ?? operatingHours.mondayIsWorking;

                        // Tuesday
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.TuesdayOpenAt))
                        {
                            operatingHours.tuesdayOpenAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.TuesdayOpenAt);
                        }
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.TuesdayCloseAt))
                        {
                            operatingHours.tuesdayCloseAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.TuesdayCloseAt);
                        }
                        operatingHours.tuesdayIsWorking = facilityDto.FacilityOperatingHours.TuesdayIsWorking ?? operatingHours.tuesdayIsWorking;

                        // Wednesday
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.WednesdayOpenAt))
                        {
                            operatingHours.wednesdayOpenAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.WednesdayOpenAt);
                        }
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.WednesdayCloseAt))
                        {
                            operatingHours.wednesdayCloseAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.WednesdayCloseAt);
                        }
                        operatingHours.wednesdayIsWorking = facilityDto.FacilityOperatingHours.WednesdayIsWorking ?? operatingHours.wednesdayIsWorking;

                        // Thursday
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.ThursdayOpenAt))
                        {
                            operatingHours.thursdayOpenAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.ThursdayOpenAt);
                        }
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.ThursdayCloseAt))
                        {
                            operatingHours.thursdayCloseAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.ThursdayCloseAt);
                        }
                        operatingHours.thursdayIsWorking = facilityDto.FacilityOperatingHours.ThursdayIsWorking ?? operatingHours.thursdayIsWorking;

                        // Friday
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.FridayOpenAt))
                        {
                            operatingHours.fridayOpenAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.FridayOpenAt);
                        }
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.FridayCloseAt))
                        {
                            operatingHours.fridayCloseAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.FridayCloseAt);
                        }
                        operatingHours.fridayIsWorking = facilityDto.FacilityOperatingHours.FridayIsWorking ?? operatingHours.fridayIsWorking;

                        // Saturday
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.SaturdayOpenAt))
                        {
                            operatingHours.saturdayOpenAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.SaturdayOpenAt);
                        }
                        if (!string.IsNullOrEmpty(facilityDto.FacilityOperatingHours.SaturdayCloseAt))
                        {
                            operatingHours.saturdayCloseAt = TimeOnly.Parse(facilityDto.FacilityOperatingHours.SaturdayCloseAt);
                        }
                        operatingHours.saturdayIsWorking = facilityDto.FacilityOperatingHours.SaturdayIsWorking ?? operatingHours.saturdayIsWorking;

                        // Update the repository
                        _unitOfWork.Repository<NewFacilityOperatingHours>().Update(operatingHours);
                    }
                }


                // Update facility services
                if (facilityDto.FacilityServices is not null && facilityDto.FacilityServices.Count > 0)
                {
                    facility.FacilityServices.Clear();
                    foreach (var serviceId in facilityDto.FacilityServices)
                    {
                        facility.FacilityServices.Add(new FacilityServices { CoreFacilityServiceID = serviceId, FacilityID = facility.Id });
                    }
                }

                // Update amenities
                if (facilityDto.FacilityAmenities is not null && facilityDto.FacilityAmenities.Count > 0)
                {
                    facility.FacilityAmenities.Clear();
                    foreach (var amenityId in facilityDto.FacilityAmenities)
                    {
                        facility.FacilityAmenities.Add(new NewFacilityAmenities { AmenityID = amenityId, FacilityId = facility.Id });
                    }
                }

                // Update accessibility options
                if (facilityDto.FacilityAccessibilityOptions is not null && facilityDto.FacilityAccessibilityOptions.Count > 0)
                {
                    facility.FacilityAccessibilityOptions.Clear();
                    foreach (var optionId in facilityDto.FacilityAccessibilityOptions)
                    {
                        facility.FacilityAccessibilityOptions.Add(new NewFacilityAccessibilityOptions { AccessibilityOptionID = optionId, FacilityID = facility.Id });
                    }
                }

                // Update sports list
                if (facilityDto.SportsList is not null && facilityDto.SportsList.Count > 0)
                {
                    facility.SportsList.Clear();
                    foreach (var sportId in facilityDto.SportsList)
                    {
                        facility.SportsList.Add(new NewFacilitySports { SportID = sportId, FacilityID = facility.Id });
                    }
                }

                _unitOfWork.Repository<NewFacility>().Update(facility);
                await _unitOfWork.Save();

                var updatedFacilityDto = new EditFacilityDto
                {
                    FacilityId = facility.Id,
                    FacilityName = facility.FacilityName,
                    FacilityTypeId = facility.FacilityTypeId,
                    FacilityPremisesTypeID = facility.FacilityPremisesTypeID,
                    Description = facility.Description,
                    FacilityLocation = facility.FacilityLocation != null ? new FacilityLocationDto
                    {
                        Latitude = facility.FacilityLocation.Latitude,
                        Longitude = facility.FacilityLocation.Longitude,
                        Name = facility.FacilityLocation.Name
                    } : null,
                    FacilityServices = facility.FacilityServices.Select(x => x.CoreFacilityServiceID).ToList(),
                    FacilityAmenities = facility.FacilityAmenities.Select(x => x.AmenityID).ToList(),
                    FacilityAccessibilityOptions = facility.FacilityAccessibilityOptions.Select(x => x.AccessibilityOptionID).ToList(),
                    SportsList = facility.SportsList.Select(x => x.SportID).ToList()
                };

                return new ResponseHandler().Success(updatedFacilityDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EditFacilityDto>("Something went wrong");
            }
        }
        public async Task<Response<EditFacilityDetailsDto>> GetFacilityEditDetailsByIdAsync(int facilityID)
        {
            try
            {
                var facilityDetails = await _facilityRepository.GetFacilityEditDetailsByIdAsync(facilityID);

                if (facilityDetails is null)
                {
                    return new ResponseHandler().NotFound<EditFacilityDetailsDto>("Facility details not found");
                }

                var tasks= facilityDetails.FacilityUploads.Select(async image =>
                {
                    image.ImageUrl=await DocumentSetting.GetPreSignedUrlAsync(image.ImageUrl);
                });
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(facilityDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EditFacilityDetailsDto>("Something went wrong");
            }
        }

        public async Task<Response<bool>> AddFacilityNewMedia(List<IFormFile> newMedia, int facilityID)
        {
            try
            {
                var facility = await _facilityRepository.GetByIdAsync(facilityID);

                if (facility is null)
                {
                    return new ResponseHandler().NotFound<bool>("Facility not found");
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

                            await _unitOfWork.Repository<FacilityImages>().Add(
                                new FacilityImages
                                {
                                    ImageUrl = url,
                                    FacilityID = facilityID
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

        public async Task<Response<bool>> UpdateFacilityOperatingHoursAsync(EditFacilityOperatingHoursDto facilityDto)
        {
            var facilityOperatingHours = await _facilityOperatingHoursRepository.GetFacilityOperatingHoursByFacilityIdAsync(facilityDto.FacilityID);

            if (facilityOperatingHours == null)
                return new ResponseHandler().NotFound<bool>("Facility operating hours not found");

            TimeOnly? ParseTime(string time)
            {
                return TimeOnly.TryParse(time, out var parsedTime) ? parsedTime : null;
            }

            // Sunday

            var sundayOpenAt = ParseTime(facilityDto.SundayOpenAt);
            var sundayCloseAt = ParseTime(facilityDto.SundayCloseAt);

            if (sundayOpenAt.HasValue && sundayCloseAt.HasValue && sundayOpenAt >= sundayCloseAt)
            {
                throw new ArgumentException("Sunday opening time must be earlier than closing time.");
            }

            facilityOperatingHours.sundayOpenAt = sundayOpenAt;
            facilityOperatingHours.sundayCloseAt = sundayCloseAt;
            facilityOperatingHours.sundayIsWorking = facilityDto.SundayIsWorking;

            // Monday

            var mondayOpenAt = ParseTime(facilityDto.MondayOpenAt);
            var mondayCloseAt = ParseTime(facilityDto.MondayCloseAt);

            if (mondayOpenAt.HasValue && mondayCloseAt.HasValue && mondayOpenAt >= mondayCloseAt)
            {
                throw new ArgumentException("Monday opening time must be earlier than closing time.");
            }

            facilityOperatingHours.mondayOpenAt = mondayOpenAt;
            facilityOperatingHours.mondayCloseAt = mondayCloseAt;
            facilityOperatingHours.mondayIsWorking = facilityDto.MondayIsWorking;

            // Tuesday

            var tuesdayOpenAt = ParseTime(facilityDto.TuesdayOpenAt);
            var tuesdayCloseAt = ParseTime(facilityDto.TuesdayCloseAt);

            if (tuesdayOpenAt.HasValue && tuesdayCloseAt.HasValue && tuesdayOpenAt >= tuesdayCloseAt)
            {
                throw new ArgumentException("Tuesday opening time must be earlier than closing time.");
            }

            facilityOperatingHours.tuesdayOpenAt = tuesdayOpenAt;
            facilityOperatingHours.tuesdayCloseAt = tuesdayCloseAt;
            facilityOperatingHours.tuesdayIsWorking = facilityDto.TuesdayIsWorking;

            // Wednesday

            var wednesdayOpenAt = ParseTime(facilityDto.WednesdayOpenAt);
            var wednesdayCloseAt = ParseTime(facilityDto.WednesdayCloseAt);

            if (wednesdayOpenAt.HasValue && wednesdayCloseAt.HasValue && wednesdayOpenAt >= wednesdayCloseAt)
            {
                throw new ArgumentException("Wednesday opening time must be earlier than closing time.");
            }

            facilityOperatingHours.wednesdayOpenAt = wednesdayOpenAt;
            facilityOperatingHours.wednesdayCloseAt = wednesdayCloseAt;
            facilityOperatingHours.wednesdayIsWorking = facilityDto.WednesdayIsWorking;

            // Thursday

            var thursdayOpenAt = ParseTime(facilityDto.ThursdayOpenAt);
            var thursdayCloseAt = ParseTime(facilityDto.ThursdayCloseAt);

            if (thursdayOpenAt.HasValue && thursdayCloseAt.HasValue && thursdayOpenAt >= thursdayCloseAt)
            {
                throw new ArgumentException("Thursday opening time must be earlier than closing time.");
            }

            facilityOperatingHours.thursdayOpenAt = thursdayOpenAt;
            facilityOperatingHours.thursdayCloseAt = thursdayCloseAt;
            facilityOperatingHours.thursdayIsWorking = facilityDto.ThursdayIsWorking;

            // Friday
            var fridayOpenAt = ParseTime(facilityDto.FridayOpenAt);
            var fridayCloseAt = ParseTime(facilityDto.FridayCloseAt);

            if (fridayOpenAt.HasValue && fridayCloseAt.HasValue && fridayOpenAt >= fridayCloseAt)
            {
                throw new ArgumentException("Friday opening time must be earlier than closing time.");
            }

            facilityOperatingHours.fridayOpenAt = fridayOpenAt;
            facilityOperatingHours.fridayCloseAt = fridayCloseAt;
            facilityOperatingHours.fridayIsWorking = facilityDto.FridayIsWorking;

            // Saturday

            var saturdayOpenAt = ParseTime(facilityDto.SaturdayOpenAt);
            var saturdayCloseAt = ParseTime(facilityDto.SaturdayCloseAt);

            if (saturdayOpenAt.HasValue && saturdayCloseAt.HasValue && saturdayOpenAt >= saturdayCloseAt)
            {
                throw new ArgumentException("Saturday opening time must be earlier than closing time.");
            }

            facilityOperatingHours.saturdayOpenAt = saturdayOpenAt;
            facilityOperatingHours.saturdayCloseAt = saturdayCloseAt;
            facilityOperatingHours.saturdayIsWorking = facilityDto.SaturdayIsWorking;

            await _unitOfWork.Save();

            return new ResponseHandler().Success(true);
        }
        public async Task<Response<GetFacilityMobileDetailsDto>> GetMobileFacilityDetailsAsync(int facilityID)
        {
            try
            {
                var facilityDetails = await _facilityRepository.GetMobileFacilityDetailsAsync(facilityID);

                if (facilityDetails is null)
                {
                    return new ResponseHandler().NotFound<GetFacilityMobileDetailsDto>("Venue details not found");
                }

                var tasks= facilityDetails.ImageUrls.Select(async imageUrl =>
                {
                    return await DocumentSetting.GetPreSignedUrlAsync(imageUrl);
                });

                facilityDetails.ImageUrls = (await Task.WhenAll(tasks)).ToList();

                return new ResponseHandler().Success(facilityDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetFacilityMobileDetailsDto>("Something went wrong");
            }
        }

        public async Task<Response<List<MobileFacilityVenuesDto>>> GetMobileFacilityVenues(int facilityID)
        {
            try
            {
                var facilityList = await _facilityRepository.GetMobileFacilityVenuesAsync(facilityID);

                if (facilityList is null)
                {
                    return new ResponseHandler().NotFound<List<MobileFacilityVenuesDto>>("Branches facilities not found");
                }

                var tasks= facilityList.Select(async facility =>
                {
                    facility.ImageURL=await DocumentSetting.GetPreSignedUrlAsync(facility.ImageURL);
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(facilityList);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MobileFacilityVenuesDto>>("Something went wrong");
            }
        }
        public async Task<Response<List<NewFacilityDashboardDto>>> GetFacilitiesByBranchID(int branchID)
        {
            try
            {
                var facilityList = await _facilityRepository.GetFaciliesByBranchID(branchID);

                if (facilityList is null)
                {
                    return new ResponseHandler().NotFound<List<NewFacilityDashboardDto>>("Branch facilities not found");
                }

                return new ResponseHandler().Success(facilityList);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<NewFacilityDashboardDto>>("Something went wrong");
            }
        }

        public async Task<Response<PaginationToReturnDto<NewFacilityDashboardDto>>> GetFacilitiesByBranch(int branchId, NewFacilityDashboardSearchDto dto)
        {
            try
            {
                if (branchId <= 0)
                {
                    Log.Error("Invalid BranchId: {BranchId}", branchId);
                    return new ResponseHandler()
                        .BadRequest<PaginationToReturnDto<NewFacilityDashboardDto>>("Invalid BranchId.");
                }

                Expression<Func<NewFacility, bool>> criteria = f =>
                f.BranchId == branchId &&
                (string.IsNullOrEmpty(dto.SearchTerm) ||
                f.FacilityName.Trim().ToLower().Contains(dto.SearchTerm.Trim().ToLower()));


                Expression<Func<NewFacility, NewFacilityDashboardDto>> selector = f => new NewFacilityDashboardDto
                {
                    Id = f.Id,
                    name = f.FacilityName
                };

                var totalCount = await _unitOfWork
                    .Repository<NewFacility>()
                    .GetCountByCriteriaAsync(criteria);

                var spec = new GeneralSpecifications<NewFacility>(criteria, dto.PageIndex, dto.PageSize);
                spec.SetOrderByDescending(f => f.Id);
                spec.AsNoTracking = true;


                var data = await _unitOfWork
                 .Repository<NewFacility>()
                 .GetAllWithSpecificationsAndProjection(spec, selector);

                Log.Information(
                    "Fetched {Count} facilities for BranchId {BranchId}",
                    data.Count(),
                    branchId
                );

                int totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

                var result = new PaginationToReturnDto<NewFacilityDashboardDto>(
                    totalCount,
                    totalPages,
                    dto.PageSize,
                    dto.PageIndex,
                    data
                );

                return new ResponseHandler().Success(result);

            }
            catch (Exception ex) {
                Log.Error(ex, "Error while fetching facilities for BranchId {BranchId}", branchId);
                return new ResponseHandler().BadRequest<PaginationToReturnDto<NewFacilityDashboardDto>>("Something went wrong.");
            }
        }

        public async Task<int> GetTotalNumberOfFacilitiesByTenantId(int tenantId)
        {
            var count = await _unitOfWork.Repository<NewFacility>()
                .GetCountByCriteriaAsync(f => f.GlobalTenantId == tenantId);
            
            return count;
        }

    }
}
