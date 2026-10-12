using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Entity.Entities;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.EventDtos;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.NewVenueRepositoy;
using MyEntityMembershipService.Repositories.Entity.VenueBookingRepo;
using MyEntityMembershipService.Repositories.Entity.VenueBookingSlotRepo;
using MyEntityMembershipService.Repositories.Entity.VenueConfigurationRepo;
using MyEntityMembershipService.Repositories.Entity.VenueOperatingHoursRepo;
using MyEntityMembershipService.Repositories.Entity.VenueRepositoy;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Entity.BranchService;
using MyEntityMembershipService.SpecificationPattern;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using MyEntityMembershipService.Dtos;
using Serilog;

namespace MyEntityMembershipService.Services.Entity.VenueService
{
    public class VenueService : IVenueService
    {
        private readonly IBranchService _branchService;
        private readonly IVenueRepository _venueRepository;
        private readonly INewVenueRepository _newVenueRepository;
        private readonly IVenueConfigurationRepository _venueConfigurationRepository;
        private readonly IVenueOperatingHoursRepository _venueOperatingHoursRepository;
        private readonly IVenueBookingSlotRepository _venueBookingSlotRepository;
        private readonly IVenueBookingRepository _venueBookingRepository;

        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;



        public VenueService(IUnitOfWork unitOfWork, IBranchService branchService, IHttpContextAccessor httpContextAccessor, IVenueBookingSlotRepository venueBookingSlotRepository, IVenueBookingRepository venueBookingRepository,
        IHttpClientFactory httpClientFactory, IVenueConfigurationRepository venueConfigurationRepository, IVenueOperatingHoursRepository venueOperatingHoursRepository, IVenueRepository venueRepository, INewVenueRepository newVenueRepository, IConfiguration configuration)
        {
            _branchService = branchService;
            _venueRepository = venueRepository;
            _newVenueRepository = newVenueRepository;
            _venueConfigurationRepository = venueConfigurationRepository;
            _venueOperatingHoursRepository = venueOperatingHoursRepository;
            _venueBookingSlotRepository = venueBookingSlotRepository;
            _venueBookingRepository = venueBookingRepository;
            _unitOfWork = unitOfWork;
            this.configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;

        }
        #region my entity venue booking
        public async Task<Response<bool>> IsVenueIdsValidForSpecificTenant(int tenantId, List<int> venues)
        {
            try
            {
                var Id = await _branchService.GetVenuesOfTenantByIdAsync(tenantId);


                var result = venues.All(v => Id.Data.Contains(v));

                if (result)
                    return new ResponseHandler().Success(result);

                else
                    return new ResponseHandler().NotFound<bool>("Invalid venusId");
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }



        }
        public async Task<Response<List<GetVenueDto>>> GetVenuesByListOfIds(List<int> venuesIds)
        {
            try
            {
                var venues = await _newVenueRepository.GetVenuesByListOfIdsAsync(venuesIds);


                var tasks = venues.Select(async venue =>
                {
                    venue.Image = await DocumentSetting.GetPreSignedUrlAsync(venue.Image);

                    venue.BranchImage = await DocumentSetting.GetPreSignedUrlAsync(venue.BranchImage);

                    venue.FacilityImage = await DocumentSetting.GetPreSignedUrlAsync(venue.FacilityImage);

                });

                await Task.WhenAll(tasks);

                if (venues.Count > 0)
                    return new ResponseHandler().Success(venues);
                else
                    return new ResponseHandler().NotFound<List<GetVenueDto>>();
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<List<GetVenueDto>>("Something went wrong");
            }

        }
        public async Task<Response<List<GetVenueWrapperDto>>> GetVenuesInfoByListOfIds(List<int> venuesIds)
        {
            try
            {
                var venues = await _newVenueRepository.GetVenuesInfoByListOfIdsAsync(venuesIds);

                var tasks = venues.Select(async venueWrapper =>
                {
                    if (venueWrapper.Venue != null)
                    {
                        venueWrapper.Venue.Image = await DocumentSetting.GetPreSignedUrlAsync(venueWrapper.Venue.Image);

                        if(venueWrapper.Venue.Branch!=null)
                        venueWrapper.Venue.Branch.Image = await DocumentSetting.GetPreSignedUrlAsync(venueWrapper.Venue.Branch.Image);
                        
                        if (venueWrapper.Venue.Facility != null)
                            venueWrapper.Venue.Facility.Image = await DocumentSetting.GetPreSignedUrlAsync(venueWrapper.Venue.Facility.Image);
                    }



                });

                await Task.WhenAll(tasks);
                if (venues.Count > 0)
                    return new ResponseHandler().Success(venues);
                else
                    return new ResponseHandler().NotFound<List<GetVenueWrapperDto>>();
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<List<GetVenueWrapperDto>>("Something went wrong");
            }

        }
        public async Task<Response<List<NewVenue>>> GetVenuesByName(string venueName)
        {
            try
            {
                var venues = await _newVenueRepository.GetVenuesByNameAsync(venueName);

                var tasks = venues.Select(async venue =>
                {
                    venue.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(venue.VenueImageURL);
                });
                await Task.WhenAll(tasks);

                if (venues.Count > 0)
                    return new ResponseHandler().Success(venues);
                else
                    return new ResponseHandler().NotFound<List<NewVenue>>();
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<List<NewVenue>>("Something went wrong");
            }

        }
        public async Task<Response<AddNewVenueDto>> CreateNewVenue(AddNewVenueDto venueDto)
        {
            try
            {
                NewVenue venue = new NewVenue
                {
                    Name = venueDto.VenueName,
                    VenueCapacity = venueDto.VenueCapacity,
                    AllowMultipleEventsAtSameTime = venueDto.AllowMultipleEventsAtSameTime,
                    VenueLocation = new VenueLocation
                    {
                        Name = venueDto.VenueLocation.Name,
                        Latitude = venueDto.VenueLocation.Latitude,
                        Longitude = venueDto.VenueLocation.Longitude
                    },
                    Description = venueDto.Description,
                    FieldDimensions = venueDto.FieldDimensions,
                    FacilityId = venueDto.FacilityID,
                };

                if (venueDto.VenueImg != null && venueDto.VenueImg.Length > 0)
                {
                    var venueImageURL = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", venueDto.VenueImg);
                    venue.VenueImageURL = venueImageURL;
                }

                await _unitOfWork.Repository<NewVenue>().Add(venue);
                await _unitOfWork.Save();

                venueDto.CreatedVenueID = venue.Id;

                NewVenueTypes venueType = new NewVenueTypes
                {
                    VenueID = venue.Id,
                    TypeID = venueDto.VenueType
                };

                await _unitOfWork.Repository<NewVenueTypes>().Add(venueType);
                await _unitOfWork.Save();

                if (venueDto.VenueUploads != null && venueDto.VenueUploads.Count > 0)
                {
                    foreach (var file in venueDto.VenueUploads)
                    {
                        if (file.Length > 0)
                        {
                            var url = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", file);
                            await _unitOfWork.Repository<VenueImages>().Add(new VenueImages { ImageUrl = url, VenueID = venue.Id });
                        }
                    }
                }

                foreach (var utility in venueDto.VenueUtilities)
                {
                    NewVenueUtilities venueUtilities = new NewVenueUtilities
                    {
                        VenueID = venue.Id,
                        UtilityID = utility
                    };

                    await _unitOfWork.Repository<NewVenueUtilities>().Add(venueUtilities);
                    await _unitOfWork.Save();
                }

                foreach (var accessibilityOption in venueDto.VenueAccessibilityOptions)
                {
                    NewVenueAccessibilityOptions option = new NewVenueAccessibilityOptions
                    {
                        VenueID = venue.Id,
                        AccessibilityOptionID = accessibilityOption
                    };

                    await _unitOfWork.Repository<NewVenueAccessibilityOptions>().Add(option);
                    await _unitOfWork.Save();
                }

                foreach (var venueAmenity in venueDto.VenueAmenities)
                {
                    NewVenueAmenities option = new NewVenueAmenities
                    {
                        VenueID = venue.Id,
                        AmenityID = venueAmenity
                    };

                    await _unitOfWork.Repository<NewVenueAmenities>().Add(option);
                    await _unitOfWork.Save();
                }

                foreach (var turfTpe in venueDto.TurfTypes)
                {
                    NewVenueTurfTypes option = new NewVenueTurfTypes
                    {
                        VenueID = venue.Id,
                        TurfTypeID = turfTpe
                    };

                    await _unitOfWork.Repository<NewVenueTurfTypes>().Add(option);
                    await _unitOfWork.Save();
                }


                return new ResponseHandler().Success(venueDto);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AddNewVenueDto>("Something went wrong");
            }

        }
        public async Task<Response<bool>> AddVenueOperatingHoursAsync(VenueOperatingHoursDto operatingHoursDto)
        {
            try
            {
                var operatingHours = new VenueOperatingHours
                {
                    NewVenueId = operatingHoursDto.VenueID,

                    sundayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.SundayOpenAt) ? TimeOnly.Parse(operatingHoursDto.SundayOpenAt) : null,
                    sundayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.SundayCloseAt) ? TimeOnly.Parse(operatingHoursDto.SundayCloseAt) : null,
                    sundayIsWorking = operatingHoursDto.SundayIsWorking ?? false,

                    mondayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.MondayOpenAt) ? TimeOnly.Parse(operatingHoursDto.MondayOpenAt) : null,
                    mondayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.MondayCloseAt) ? TimeOnly.Parse(operatingHoursDto.MondayCloseAt) : null,
                    mondayIsWorking = operatingHoursDto.MondayIsWorking ?? false,

                    tuesdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.TuesdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.TuesdayOpenAt) : null,
                    tuesdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.TuesdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.TuesdayCloseAt) : null,
                    tuesdayIsWorking = operatingHoursDto.TuesdayIsWorking ?? false,

                    wednesdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.WednesdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.WednesdayOpenAt) : null,
                    wednesdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.WednesdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.WednesdayCloseAt) : null,
                    wednesdayIsWorking = operatingHoursDto.WednesdayIsWorking ?? false,

                    thursdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.ThursdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.ThursdayOpenAt) : null,
                    thursdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.ThursdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.ThursdayCloseAt) : null,
                    thursdayIsWorking = operatingHoursDto.ThursdayIsWorking ?? false,

                    fridayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.FridayOpenAt) ? TimeOnly.Parse(operatingHoursDto.FridayOpenAt) : null,
                    fridayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.FridayCloseAt) ? TimeOnly.Parse(operatingHoursDto.FridayCloseAt) : null,
                    fridayIsWorking = operatingHoursDto.FridayIsWorking ?? false,

                    saturdayOpenAt = !string.IsNullOrEmpty(operatingHoursDto.SaturdayOpenAt) ? TimeOnly.Parse(operatingHoursDto.SaturdayOpenAt) : null,
                    saturdayCloseAt = !string.IsNullOrEmpty(operatingHoursDto.SaturdayCloseAt) ? TimeOnly.Parse(operatingHoursDto.SaturdayCloseAt) : null,
                    saturdayIsWorking = operatingHoursDto.SaturdayIsWorking ?? false
                };

                await _unitOfWork.Repository<VenueOperatingHours>().Add(operatingHours);
                await _unitOfWork.Save();


                // await GenerateAndSaveSlotsForWorkingDaysAsync(venueConfiguration, operatingHours, operatingHoursDto.VenueID);

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<GetVenuePaginationListDto>> GetVenueListByFacilityID(int pageNumber, int facilityID)
        {
            try
            {
                var venueList = await _newVenueRepository.GetVenuesPaginatedAsync(pageNumber, facilityID);

                var tasks = venueList.Venues.Select(async venue =>
                {
                    venue.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(venue.ImageURL);


                });

                await Task.WhenAll(tasks);


                if (venueList is null)
                {
                    return new ResponseHandler().NotFound<GetVenuePaginationListDto>("Venue list not found");
                }

                return new ResponseHandler().Success(venueList);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenuePaginationListDto>("Something went wrong");
            }
        }
        public async Task<Response<GetVenuePaginationListDto>> GetMobileVenueListByFacilityID(int pageNumber, int facilityID)
        {
            try
            {
                var venueList = await _newVenueRepository.GetMobileVenuesPaginatedAsync(pageNumber, facilityID);

                if (venueList is null)
                {
                    return new ResponseHandler().NotFound<GetVenuePaginationListDto>("Venue list not found");
                }

                var tasks= venueList.Venues.Select(async venue =>
                {
                    venue.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(venue.ImageURL);
                });
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(venueList);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenuePaginationListDto>("Something went wrong");
            }
        }
        public async Task<Response<GetVenueDetailsDto>> GetVenueDetailsByIdAsync(int venueID)
        {
            try
            {
                var branchDetails = await _newVenueRepository.GetVenueDetailsAsync(venueID);

                if (branchDetails is null)
                {
                    return new ResponseHandler().NotFound<GetVenueDetailsDto>("Venue details not found");
                }

                var tasks= branchDetails.VenueImages.Select(async venueImage =>
                {
                    branchDetails.HeaderDetails.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDetails.HeaderDetails.BranchImageURL);
                    branchDetails.HeaderDetails.VanueImageURL = await DocumentSetting.GetPreSignedUrlAsync(branchDetails.HeaderDetails.VanueImageURL);
                    venueImage.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(venueImage.ImageUrl);
                });
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(branchDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenueDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<bool>> RemoveVenueAsync(int venueId)
        {
            try
            {
                var venue = await _unitOfWork.Repository<NewVenue>().GetWithIncludesAsync(
                    venueId,
                    v => v.VenueLocation,
                    v => v.OperatingHours,
                    v => v.VenueUtilities,
                    v => v.VenueAccessibilityOptions,
                    v => v.VenueAmenities,
                    v => v.TurfTypes,
                    v => v.VenueUploads);

                if (venue == null)
                {
                    return new ResponseHandler().NotFound<bool>("Venue not found.");
                }

                var venueCheckDto = new CreationCheckDto
                {
                    VenueId = venueId,
                    TenantId = venue.Facility.Branch.TenantId
                };

                var isLinked = await LinkageHelper.IsLinkedToProgramAsync(venueCheckDto, _httpClientFactory, _httpContextAccessor);

                if (venue.VenueLocation != null)
                {
                    _unitOfWork.Repository<VenueLocation>().SoftDelete(venue.VenueLocation);
                }

                if (venue.OperatingHours != null)
                {
                    _unitOfWork.Repository<VenueOperatingHours>().SoftDelete(venue.OperatingHours);
                }

                if (venue.VenueUtilities != null && venue.VenueUtilities.Any())
                {
                    foreach (var utility in venue.VenueUtilities)
                    {
                        _unitOfWork.Repository<NewVenueUtilities>().SoftDelete(utility);
                    }
                }

                if (venue.VenueAccessibilityOptions != null && venue.VenueAccessibilityOptions.Any())
                {
                    foreach (var accessibilityOption in venue.VenueAccessibilityOptions)
                    {
                        _unitOfWork.Repository<NewVenueAccessibilityOptions>().SoftDelete(accessibilityOption);
                    }
                }

                if (venue.VenueAmenities != null && venue.VenueAmenities.Any())
                {
                    foreach (var amenity in venue.VenueAmenities)
                    {
                        _unitOfWork.Repository<NewVenueAmenities>().SoftDelete(amenity);
                    }
                }

                if (venue.TurfTypes != null && venue.TurfTypes.Any())
                {
                    foreach (var turfType in venue.TurfTypes)
                    {
                        _unitOfWork.Repository<NewVenueTurfTypes>().SoftDelete(turfType);
                    }
                }

                if (venue.VenueUploads != null && venue.VenueUploads.Any())
                {
                    foreach (var upload in venue.VenueUploads)
                    {
                        _unitOfWork.Repository<VenueImages>().SoftDelete(upload);
                    }
                }

                _unitOfWork.Repository<NewVenue>().SoftDelete(venue);

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true, "Venue and all related entities successfully deleted.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<bool>> RemoveVenueMediaAsync(int venueID, int mediaId)
        {
            try
            {
                var venue = await _unitOfWork.Repository<NewVenue>().GetWithIncludesAsync(
                    venueID,
                    b => b.VenueUploads);

                if (venue == null)
                {
                    return new ResponseHandler().NotFound<bool>("Venue not found.");
                }

                var venueImage = venue.VenueUploads.FirstOrDefault(bi => bi.Id == mediaId);

                if (venueImage == null)
                {
                    return new ResponseHandler().NotFound<bool>("Media not found.");
                }

                _unitOfWork.Repository<VenueImages>().SoftDelete(venueImage);

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true, "Media successfully deleted from the facility.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<EditVenueDto>> EditVenueAsync(EditVenueDto venueDto)
        {
            try
            {
                var venue = await _unitOfWork.Repository<NewVenue>().GetWithIncludesAsync(
                    venueDto.VenueId,
                    v => v.VenueLocation,
                    v => v.OperatingHours,
                    v => v.VenueUtilities,
                    v => v.VenueAccessibilityOptions,
                    v => v.VenueAmenities,
                    v => v.TurfTypes,
                    v => v.VenueUploads);

                if (venue == null)
                {
                    return new ResponseHandler().NotFound<EditVenueDto>("Venue not found");
                }

                if (venueDto.VenueImageURL != null && venueDto.VenueImageURL.Length > 0)
                {
                    var venueImageURL = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", venueDto.VenueImageURL);
                    venue.VenueImageURL = venueImageURL;
                }

                // Update basic properties
                venue.Name = venueDto.Name ?? venue.Name;
                venue.VenueCapacity = venueDto.VenueCapacity != 0 ? venueDto.VenueCapacity : venue.VenueCapacity;
                venue.Description = !string.IsNullOrEmpty(venueDto.Description) ? venueDto.Description : venue.Description;
                venue.FieldDimensions = venueDto.FieldDimensions ?? venue.FieldDimensions;
                venue.AllowMultipleEventsAtSameTime = venueDto.AllowMultipleEventsAtSameTime ?? venue.AllowMultipleEventsAtSameTime;

                // Handle file uploads
                if (venueDto.VenueUploads is not null && venueDto.VenueUploads.Count > 0)
                {
                    foreach (var file in venueDto.VenueUploads)
                    {
                        if (file.Length > 0)
                        {
                            var url = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", file);
                            venue.VenueImageURL = url;
                        }
                    }
                }

                // Update location
                if (venueDto.VenueLocation is not null)
                {
                    var venueLocation = await _unitOfWork.Repository<VenueLocation>().GetByIdAsync(venue.VenueLocation.Id);
                    if (venueLocation is not null)
                    {
                        venueLocation.Latitude = venueDto.VenueLocation.Latitude;
                        venueLocation.Longitude = venueDto.VenueLocation.Longitude;
                        venueLocation.Name = venueDto.VenueLocation.Name;
                        _unitOfWork.Repository<VenueLocation>().Update(venueLocation);
                    }
                }

                // Update utilities
                if (venueDto.VenueUtilities is not null && venueDto.VenueUtilities.Count > 0)
                {
                    venue.VenueUtilities.Clear();
                    foreach (var utilityId in venueDto.VenueUtilities)
                    {
                        venue.VenueUtilities.Add(new NewVenueUtilities { UtilityID = utilityId, VenueID = venue.Id });
                    }
                }

                // Update amenities
                if (venueDto.VenueAmenities is not null && venueDto.VenueAmenities.Count > 0)
                {
                    venue.VenueAmenities.Clear();
                    foreach (var amenityId in venueDto.VenueAmenities)
                    {
                        venue.VenueAmenities.Add(new NewVenueAmenities { AmenityID = amenityId, VenueID = venue.Id });
                    }
                }

                // Update accessibility options
                if (venueDto.VenueAccessibilityOptions is not null && venueDto.VenueAccessibilityOptions.Count > 0)
                {
                    venue.VenueAccessibilityOptions.Clear();
                    foreach (var optionId in venueDto.VenueAccessibilityOptions)
                    {
                        venue.VenueAccessibilityOptions.Add(new NewVenueAccessibilityOptions { AccessibilityOptionID = optionId, VenueID = venue.Id });
                    }
                }

                // Update turf types
                if (venueDto.TurfTypes is not null && venueDto.TurfTypes.Count > 0)
                {
                    venue.TurfTypes.Clear();
                    foreach (var turfTypeId in venueDto.TurfTypes)
                    {
                        venue.TurfTypes.Add(new NewVenueTurfTypes { TurfTypeID = turfTypeId, VenueID = venue.Id });
                    }
                }

                // Save changes
                _unitOfWork.Repository<NewVenue>().Update(venue);
                await _unitOfWork.Save();

                // Create response DTO
                var updatedVenueDto = new EditVenueDto
                {
                    VenueId = venue.Id,
                    Name = venue.Name,
                    VenueCapacity = venue.VenueCapacity,
                    VenueLocation = venue.VenueLocation != null ? new VenueLocationDto
                    {
                        Latitude = venue.VenueLocation.Latitude,
                        Longitude = venue.VenueLocation.Longitude,
                        Name = venue.VenueLocation.Name
                    } : null,
                    VenueUtilities = venue.VenueUtilities.Select(x => x.UtilityID).ToList(),
                    VenueAmenities = venue.VenueAmenities.Select(x => x.AmenityID).ToList(),
                    VenueAccessibilityOptions = venue.VenueAccessibilityOptions.Select(x => x.AccessibilityOptionID).ToList(),
                    TurfTypes = venue.TurfTypes.Select(x => x.TurfTypeID).ToList(),
                    FieldDimensions = venue.FieldDimensions,
                    Description = venue.Description
                };

                return new ResponseHandler().Success(updatedVenueDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EditVenueDto>("Something went wrong");
            }
        }
        public async Task<Response<EditVenueDetailsDto>> GetVenueEditDetailsByIdAsync(int venueID)
        {
            try
            {
                var facilityDetails = await _venueRepository.GetVenueEditDetailsByIdAsync(venueID);

                if (facilityDetails is null)
                {
                    return new ResponseHandler().NotFound<EditVenueDetailsDto>("Facility details not found");
                }

                var tasks= facilityDetails.VenueUploads.Select(async venueImage =>
                {
                    venueImage.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(venueImage.ImageUrl);
                });
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(facilityDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EditVenueDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<GetVenueMobileDetailsDto>> GetMobileVenueDetailsAsync(int VenueID)
        {
            try
            {
                var venueDetails = await _venueRepository.GetMobileVenueDetailsAsync(VenueID);

                if (venueDetails is null)
                {
                    return new ResponseHandler().NotFound<GetVenueMobileDetailsDto>("Venue details not found");
                }

                var tasks= venueDetails.VenueMedia.Select(async venueImage =>
                {
                    
                    venueImage.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(venueImage.ImageUrl);
                });
                venueDetails.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(venueDetails.VenueImageURL);
                
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(venueDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenueMobileDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<bool>> AddVenueNewMedia(List<IFormFile> newMedia, int venueID)
        {
            try
            {
                var venue = await _newVenueRepository.GetByIdAsync(venueID);

                if (venue is null)
                {
                    return new ResponseHandler().NotFound<bool>("Venue not found");
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

                            await _unitOfWork.Repository<VenueImages>().Add(
                                new VenueImages
                                {
                                    ImageUrl = url,
                                    VenueID = venueID
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

        public async Task<Response<bool>> UpdateVenueOperatingHoursAsync(EditVenueOperatingHoursDto venueDto)
        {
            var venueOperatingHours = await _venueOperatingHoursRepository.GetVenueOperatingHoursByVenueIdAsync(venueDto.VenueID);

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
            var sundayOpenAt = ParseTime(venueDto.SundayOpenAt);
            var sundayCloseAt = ParseTime(venueDto.SundayCloseAt);
            var mondayOpenAt = ParseTime(venueDto.MondayOpenAt);
            var mondayCloseAt = ParseTime(venueDto.MondayCloseAt);
            var tuesdayOpenAt = ParseTime(venueDto.TuesdayOpenAt);
            var tuesdayCloseAt = ParseTime(venueDto.TuesdayCloseAt);
            var wednesdayOpenAt = ParseTime(venueDto.WednesdayOpenAt);
            var wednesdayCloseAt = ParseTime(venueDto.WednesdayCloseAt);
            var thursdayOpenAt = ParseTime(venueDto.ThursdayOpenAt);
            var thursdayCloseAt = ParseTime(venueDto.ThursdayCloseAt);
            var fridayOpenAt = ParseTime(venueDto.FridayOpenAt);
            var fridayCloseAt = ParseTime(venueDto.FridayCloseAt);
            var saturdayOpenAt = ParseTime(venueDto.SaturdayOpenAt);
            var saturdayCloseAt = ParseTime(venueDto.SaturdayCloseAt);

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

            if (venueOperatingHours == null)
            {
                // ADD
                venueOperatingHours = new VenueOperatingHours
                {
                    VenueId = venueDto.VenueID,

                    sundayOpenAt = sundayOpenAt,
                    sundayCloseAt = sundayCloseAt,
                    sundayIsWorking = venueDto.SundayIsWorking,

                    mondayOpenAt = mondayOpenAt,
                    mondayCloseAt = mondayCloseAt,
                    mondayIsWorking = venueDto.MondayIsWorking,

                    tuesdayOpenAt = tuesdayOpenAt,
                    tuesdayCloseAt = tuesdayCloseAt,
                    tuesdayIsWorking = venueDto.TuesdayIsWorking,

                    wednesdayOpenAt = wednesdayOpenAt,
                    wednesdayCloseAt = wednesdayCloseAt,
                    wednesdayIsWorking = venueDto.WednesdayIsWorking,

                    thursdayOpenAt = thursdayOpenAt,
                    thursdayCloseAt = thursdayCloseAt,
                    thursdayIsWorking = venueDto.ThursdayIsWorking,

                    fridayOpenAt = fridayOpenAt,
                    fridayCloseAt = fridayCloseAt,
                    fridayIsWorking = venueDto.FridayIsWorking,

                    saturdayOpenAt = saturdayOpenAt,
                    saturdayCloseAt = saturdayCloseAt,
                    saturdayIsWorking = venueDto.SaturdayIsWorking,
                };

                await _venueOperatingHoursRepository.Add(venueOperatingHours);
            }
            else
            {
                // UPDATE
                venueOperatingHours.sundayOpenAt = sundayOpenAt;
                venueOperatingHours.sundayCloseAt = sundayCloseAt;
                venueOperatingHours.sundayIsWorking = venueDto.SundayIsWorking;

                venueOperatingHours.mondayOpenAt = mondayOpenAt;
                venueOperatingHours.mondayCloseAt = mondayCloseAt;
                venueOperatingHours.mondayIsWorking = venueDto.MondayIsWorking;

                venueOperatingHours.tuesdayOpenAt = tuesdayOpenAt;
                venueOperatingHours.tuesdayCloseAt = tuesdayCloseAt;
                venueOperatingHours.tuesdayIsWorking = venueDto.TuesdayIsWorking;

                venueOperatingHours.wednesdayOpenAt = wednesdayOpenAt;
                venueOperatingHours.wednesdayCloseAt = wednesdayCloseAt;
                venueOperatingHours.wednesdayIsWorking = venueDto.WednesdayIsWorking;

                venueOperatingHours.thursdayOpenAt = thursdayOpenAt;
                venueOperatingHours.thursdayCloseAt = thursdayCloseAt;
                venueOperatingHours.thursdayIsWorking = venueDto.ThursdayIsWorking;

                venueOperatingHours.fridayOpenAt = fridayOpenAt;
                venueOperatingHours.fridayCloseAt = fridayCloseAt;
                venueOperatingHours.fridayIsWorking = venueDto.FridayIsWorking;

                venueOperatingHours.saturdayOpenAt = saturdayOpenAt;
                venueOperatingHours.saturdayCloseAt = saturdayCloseAt;
                venueOperatingHours.saturdayIsWorking = venueDto.SaturdayIsWorking;


            }

            await _unitOfWork.Save();

            return new ResponseHandler().Success(true);
        }

        public async Task<Response<PaginationToReturnDto<VenueCardDetailsDto>>> GetAllVenues(VenueQueryPrametersDto queryParam)
        {
            try
            {
                var term = queryParam.SearchTerm?.Trim().ToLower();

                Expression<Func<NewVenue, bool>> criteria = x =>
                    (!queryParam.TenantId.HasValue ||
                     (x.Facility != null &&
                      x.Facility.Branch != null &&
                      x.Facility.Branch.TenantId == queryParam.TenantId.Value)) &&

                    (!queryParam.BranchId.HasValue ||
                     (x.Facility != null &&
                      x.Facility.BranchId == queryParam.BranchId.Value)) &&

                    (!queryParam.FacilityId.HasValue ||
                     x.FacilityId == queryParam.FacilityId.Value) &&

                    (string.IsNullOrWhiteSpace(term) ||
                     x.Name.ToLower().Contains(term) ||
                     (!string.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(term)));
                var specs = new GeneralSpecifications<NewVenue>(criteria,
                    queryParam.PageNumber, queryParam.PageSize
                );
                var venues = await _unitOfWork.Repository<NewVenue>().GetAllWithSpecificationsAndProjection(specs, x =>
                    new VenueCardDetailsDto
                    {
                        VenueID = x.Id,
                        VenueType = x.VenueType.VenueTypes.TypeName,
                        VenueName = x.Name,
                        ImageURL = x.VenueImageURL,
                        Rating = x.VenueReviews.Any()
                            ? Math.Round(x.VenueReviews.Average(r => r.Rating), 2)
                            : 0


                    });
                var venueCount = await _unitOfWork.Repository<NewVenue>().GetCountByCriteriaAsync(criteria);
                await Task.WhenAll(venues.Select(async v => v.ImageURL = await DocumentSetting.GetPreSignedUrlAsync(v.ImageURL)));

                var response = new PaginationToReturnDto<VenueCardDetailsDto>(venueCount,
                    venueCount > 0 && queryParam.PageSize > 0 ? venueCount / queryParam.PageSize : 0,
                    queryParam.PageSize, queryParam.PageNumber, venues);

                return new ResponseHandler().Success(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error occured while fetching venues");
                return new ResponseHandler().BadRequest<PaginationToReturnDto<VenueCardDetailsDto>>("Something went wrong");
            }

        }
        

        //Booking logic

        public async Task<Response<GetVenueBookingHeaderDetailsDto>> GetVenueBookingHeader(int venueId)
        {
            try
            {
                var venueHeaderDetails = await _venueRepository.GetVenueBookingHeaderDetailsAsync(venueId);

                if (venueHeaderDetails is null)
                {
                    return new ResponseHandler().NotFound<GetVenueBookingHeaderDetailsDto>("Venue details not found");
                }

               venueHeaderDetails.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync(venueHeaderDetails.VenueImageURL);


                return new ResponseHandler().Success(venueHeaderDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenueBookingHeaderDetailsDto>("Something went wrong");
            }
        }
        public async Task<Response<VenueOperatingHours>> GetVenueOperatingHours(int venueId)
        {
            try
            {
                var venueHeaderDetails = await _venueOperatingHoursRepository.GetVenueOperatingHoursByVenueIdAsync(venueId);

                if (venueHeaderDetails is null)
                {
                    return new ResponseHandler().NotFound<VenueOperatingHours>("Venue details not found");
                }
                return new ResponseHandler().Success(venueHeaderDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<VenueOperatingHours>("Something went wrong");
            }
        }


        public async Task<Response<List<AvailableSlotDto>>> GetAvailableSlots(GetAvailableSlotsRequestDto request)
        {
            try
            {
                DateTime date = DateTime.ParseExact(request.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DayOfWeek dayOfWeek = date.DayOfWeek;

                if (DateOnly.FromDateTime(date) < DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    return new ResponseHandler().BadRequest<List<AvailableSlotDto>>("You can not book slots for a day that has passed.");
                }

                // Fetch venue operating hours
                var venueOperatingHours = await _venueOperatingHoursRepository.GetVenueOperatingHoursByVenueIdAsync(request.VenueId);

                TimeOnly? openTime = null;
                TimeOnly? closeTime = null;
                bool? isWorking = null;

                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        openTime = venueOperatingHours.sundayOpenAt;
                        closeTime = venueOperatingHours.sundayCloseAt;
                        isWorking = venueOperatingHours.sundayIsWorking;
                        break;
                    case DayOfWeek.Monday:
                        openTime = venueOperatingHours.mondayOpenAt;
                        closeTime = venueOperatingHours.mondayCloseAt;
                        isWorking = venueOperatingHours.mondayIsWorking;
                        break;
                    case DayOfWeek.Tuesday:
                        openTime = venueOperatingHours.tuesdayOpenAt;
                        closeTime = venueOperatingHours.tuesdayCloseAt;
                        isWorking = venueOperatingHours.tuesdayIsWorking;
                        break;
                    case DayOfWeek.Wednesday:
                        openTime = venueOperatingHours.wednesdayOpenAt;
                        closeTime = venueOperatingHours.wednesdayCloseAt;
                        isWorking = venueOperatingHours.wednesdayIsWorking;
                        break;
                    case DayOfWeek.Thursday:
                        openTime = venueOperatingHours.thursdayOpenAt;
                        closeTime = venueOperatingHours.thursdayCloseAt;
                        isWorking = venueOperatingHours.thursdayIsWorking;
                        break;
                    case DayOfWeek.Friday:
                        openTime = venueOperatingHours.fridayOpenAt;
                        closeTime = venueOperatingHours.fridayCloseAt;
                        isWorking = venueOperatingHours.fridayIsWorking;
                        break;
                    case DayOfWeek.Saturday:
                        openTime = venueOperatingHours.saturdayOpenAt;
                        closeTime = venueOperatingHours.saturdayCloseAt;
                        isWorking = venueOperatingHours.saturdayIsWorking;
                        break;
                }

                if (openTime == null || closeTime == null || isWorking != true)
                {
                    return new Response<List<AvailableSlotDto>>("Venue is not open on this day.")
                    {
                        Succeeded = false,
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                DateTime startTime = DateTime.ParseExact(request.StartTime, "HH:mm", CultureInfo.InvariantCulture);
                DateTime endTime = DateTime.ParseExact(request.EndTime, "HH:mm", CultureInfo.InvariantCulture);

                DateTime venueOpenTime = DateTime.Today.Add(openTime.Value.ToTimeSpan());
                DateTime venueCloseTime = DateTime.Today.Add(closeTime.Value.ToTimeSpan());

                if (startTime < venueOpenTime) startTime = venueOpenTime;
                if (endTime > venueCloseTime) endTime = venueCloseTime;

                var venueConfiguration = await _venueConfigurationRepository.GetVenueConfigurationByVenueID(request.VenueId);
                var pricingRules = await _venueConfigurationRepository.GetPricingRulesByVenueIdAndDayAsync(request.VenueId, dayOfWeek);

                var slotDuration = TimeSpan.FromMinutes(venueConfiguration.SlotTime);
                List<AvailableSlotDto> availableSlots = new List<AvailableSlotDto>();
                DateTime currentDateTime = DateTime.Now;

                for (DateTime slotStart = startTime; slotStart < endTime; slotStart = slotStart.Add(slotDuration))
                {
                    DateTime slotEnd = slotStart.Add(slotDuration);
                    if (slotEnd <= endTime)
                    {
                        string status;
                        string price;

                        var existingBookings = await _venueBookingSlotRepository.AnyBookingOverlapAsync(
                            request.VenueId,
                            date,
                            TimeOnly.FromDateTime(slotStart),
                            TimeOnly.FromDateTime(slotEnd)
                        );

                        if (existingBookings)
                        {
                            var venueBooking = await _venueBookingRepository.GetBookingByVenueAndDate(request.VenueId, date);
                            status = venueBooking?.BookingType == 2 ? "Out of Service" : "Booked";
                        }
                        else if (date.Date == DateTime.Today && slotEnd <= currentDateTime)
                        {
                            status = "Unavailable";
                        }
                        else
                        {
                            status = "Not Booked";
                        }

                        var customPrice = await _venueConfigurationRepository.GetPriceForSlotAsync(
                            request.VenueId,
                            dayOfWeek,
                            TimeOnly.FromDateTime(slotStart),
                            TimeOnly.FromDateTime(slotEnd)
                        );
                        price = customPrice?.ToString() ?? venueConfiguration.PricePerSlot.ToString();

                        availableSlots.Add(new AvailableSlotDto
                        {
                            SlotStart = slotStart.ToString("HH:mm"),
                            SlotEnd = slotEnd.ToString("HH:mm"),
                            Status = status,
                            Price = price
                        });
                    }
                }

                return new ResponseHandler().Success(availableSlots);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<AvailableSlotDto>>("Something went wrong");
            }
        }



        public async Task<Response<bool>> BookVenueForEvent(EventBookingDto bookingDto)
        {
            try
            {
                var venueConfiguration = await _venueConfigurationRepository.GetVenueConfigurationByVenueID(bookingDto.VenueId);

                if (venueConfiguration == null)
                {
                    return new ResponseHandler().BadRequest<bool>("Venue configuration not found.");
                }

                if (bookingDto.TimeSlots.Count > venueConfiguration.MaxSlotsBooked)
                {
                    return new ResponseHandler().BadRequest<bool>($"You cannot book more than {venueConfiguration.MaxSlotsBooked} slots for this venue.");
                }

                if (bookingDto.TimeSlots.Count < venueConfiguration.MinSlotsBooked)
                {
                    return new ResponseHandler().BadRequest<bool>($"You cannot book less than {venueConfiguration.MinSlotsBooked} slots for this venue.");
                }

                foreach (var timeSlot in bookingDto.TimeSlots)
                {
                    var slotStart = TimeOnly.Parse(timeSlot.SlotStart);
                    var slotEnd = TimeOnly.Parse(timeSlot.SlotEnd);

                    var existingBookings = await _venueBookingSlotRepository.AnyBookingOverlapAsync(bookingDto.VenueId, bookingDto.BookingDate, slotStart, slotEnd);

                    if (existingBookings)
                    {
                        return new ResponseHandler().BadRequest<bool>("One or more time slots are already booked for the selected date.");
                    }
                }

                var venueBooking = new VenueBooking
                {
                    VenueId = bookingDto.VenueId,
                    BookingDate = bookingDto.BookingDate,
                    BookingType = bookingDto.BookingTypeID,
                    EventName = bookingDto.EventName,
                    Price = bookingDto.Price,
                    EventypeID = bookingDto.EventypeID,
                    Description = bookingDto.Description,
                    IsNonMember = bookingDto.IsNonMember
                };

                _unitOfWork.Repository<VenueBooking>().Add(venueBooking);
                await _unitOfWork.Save();

                foreach (var timeSlot in bookingDto.TimeSlots)
                {
                    var bookingSlot = new VenueBookingSlot
                    {
                        VenueBookingId = venueBooking.Id,
                        SlotStart = TimeOnly.Parse(timeSlot.SlotStart),
                        SlotEnd = TimeOnly.Parse(timeSlot.SlotEnd),
                    };

                    _unitOfWork.Repository<VenueBookingSlot>().Add(bookingSlot);
                }

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }



        public async Task<Response<bool>> BookVenueOutOfService(BaseBookingDto bookingDto)
        {
            try
            {
                var venueConfiguration = await _venueConfigurationRepository.GetVenueConfigurationByVenueID(bookingDto.VenueId);

                if (venueConfiguration == null)
                {
                    return new ResponseHandler().BadRequest<bool>("Venue configuration not found.");
                }

                foreach (var timeSlot in bookingDto.TimeSlots)
                {
                    var slotStart = TimeOnly.Parse(timeSlot.SlotStart);
                    var slotEnd = TimeOnly.Parse(timeSlot.SlotEnd);

                    var existingBookings = await _venueBookingSlotRepository.AnyBookingOverlapAsync(bookingDto.VenueId, bookingDto.BookingDate, slotStart, slotEnd);

                    if (existingBookings)
                    {
                        return new ResponseHandler().BadRequest<bool>("One or more time slots are already booked for the selected date.");
                    }
                }

                var venueBooking = new VenueBooking
                {
                    VenueId = bookingDto.VenueId,
                    BookingDate = bookingDto.BookingDate,
                    BookingType = bookingDto.BookingTypeID,
                };

                _unitOfWork.Repository<VenueBooking>().Add(venueBooking);
                await _unitOfWork.Save();

                foreach (var timeSlot in bookingDto.TimeSlots)
                {
                    var bookingSlot = new VenueBookingSlot
                    {
                        VenueBookingId = venueBooking.Id,
                        SlotStart = TimeOnly.Parse(timeSlot.SlotStart),
                        SlotEnd = TimeOnly.Parse(timeSlot.SlotEnd),
                    };

                    _unitOfWork.Repository<VenueBookingSlot>().Add(bookingSlot);
                }

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<bool>> BookVenueForMember(EventMemberBookingDto bookingDto)
        {
            try
            {
                var venueConfiguration = await _venueConfigurationRepository.GetVenueConfigurationByVenueID(bookingDto.VenueId);

                if (venueConfiguration == null)
                {
                    return new ResponseHandler().BadRequest<bool>("Venue configuration not found.");
                }

                if (bookingDto.TimeSlots.Count > venueConfiguration.MaxSlotsBooked)
                {
                    return new ResponseHandler().BadRequest<bool>($"You cannot book more than {venueConfiguration.MaxSlotsBooked} slots for this venue.");
                }

                if (bookingDto.TimeSlots.Count < venueConfiguration.MinSlotsBooked)
                {
                    return new ResponseHandler().BadRequest<bool>($"You cannot book less than {venueConfiguration.MinSlotsBooked} slots for this venue.");
                }

                foreach (var timeSlot in bookingDto.TimeSlots)
                {
                    var slotStart = TimeOnly.Parse(timeSlot.SlotStart);
                    var slotEnd = TimeOnly.Parse(timeSlot.SlotEnd);

                    var existingBookings = await _venueBookingSlotRepository.AnyBookingOverlapAsync(bookingDto.VenueId, bookingDto.BookingDate, slotStart, slotEnd);

                    if (existingBookings)
                    {
                        return new ResponseHandler().BadRequest<bool>("One or more time slots are already booked for the selected date.");
                    }
                }

                var venueBooking = new VenueBooking
                {
                    VenueId = bookingDto.VenueId,
                    BookingDate = bookingDto.BookingDate,
                    BookingType = bookingDto.BookingTypeID,
                    MemberID = bookingDto.MemberID,
                    Price = bookingDto.Price,
                    MemberName = bookingDto.MemberName,
                    PhoneNumber = bookingDto.PhoneNumber,
                };

                _unitOfWork.Repository<VenueBooking>().Add(venueBooking);
                await _unitOfWork.Save();

                foreach (var timeSlot in bookingDto.TimeSlots)
                {
                    var bookingSlot = new VenueBookingSlot
                    {
                        VenueBookingId = venueBooking.Id,
                        SlotStart = TimeOnly.Parse(timeSlot.SlotStart),
                        SlotEnd = TimeOnly.Parse(timeSlot.SlotEnd),
                    };

                    _unitOfWork.Repository<VenueBookingSlot>().Add(bookingSlot);
                }

                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<List<VenueBookingDto>>> GetVenuesBookings(GetVenueBookingsDto request)
        {
            try
            {
                var result = await _newVenueRepository.GetVenueBookingsAsync(request);

                if (result is null)
                {
                    return new ResponseHandler().NotFound<List<VenueBookingDto>>("No booking information found");
                }

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<VenueBookingDto>>("Something went wrong");
            }
        }

        public async Task<Response<PaginationToReturnDto<VenueBookingResponseDto>>> GetVenueBooking(VenueBookingQueryParametersDto param)
        {
            try
            {
                var specs = new GeneralSpecifications<VenueBooking>(x => x.VenueId == param.VenueId &&
                                                                         (!param.VenueType.HasValue ||
                                                                          x.BookingType == param.VenueType.Value),
                    param.PageIndex, param.PageSize);
                var venueBooking = await _unitOfWork.Repository<VenueBooking>().GetAllWithSpecificationsAndProjection(
                    specs,
                    x => new VenueBookingResponseDto
                    {
                        Date = x.BookingDate,
                        Name = x.MemberName ?? x.EventName,
                        Type = x.BookingType
                    });

                var count = await _unitOfWork.Repository<VenueBooking>().GetCountByCriteriaAsync(x =>
                    x.VenueId == param.VenueId &&
                    (!param.VenueType.HasValue || x.BookingType == param.VenueType.Value));

                var response = new PaginationToReturnDto<VenueBookingResponseDto>(count,
                    count > 0 && param.PageSize > 0 ? count / param.PageSize : 0, param.PageSize, param.PageIndex,
                    venueBooking);

                return new ResponseHandler().Success(response);


            }
            catch (Exception e)
            {
                Log.Error(e, "Error in GetVenueBooking");
                return new ResponseHandler().BadRequest<PaginationToReturnDto<VenueBookingResponseDto>>("Something went wrong");
            }
        }
        

        public async Task<Response<GetVenueBookingsMobileDto>> GetVenuesBookingsMobile(int venueID, string userId)
        {
            try
            {
                var result = await _newVenueRepository.GetVenuesBookingsMobile(venueID,userId); //FIXME: Make it user specific once auth is done

                if (result is null)
                {
                    return new ResponseHandler().NotFound<GetVenueBookingsMobileDto>("No booking information found");
                }

                result.VenueImage = await DocumentSetting.GetPreSignedUrlAsync(result.VenueImage);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenueBookingsMobileDto>("Something went wrong");
            }
        }
        public async Task<Response<string>> GetCancellationPolicyMessage(int venueID)
        {
            try
            {
                var result = await _newVenueRepository.GetCancellationPolicyMessage(venueID);

                if (result is null)
                {
                    return new ResponseHandler().NotFound<string>("No configuration information found");
                }

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>("Something went wrong");
            }
        }


        // public async Task<Response<List<VenueSlots>>> GenerateSlotsForDateAsync(int venueId, DateTime date)
        // {
        //     var venueConfiguration = await _venueConfigurationRepository.GetVenueConfigurationByVenueID(venueId);
        //     var operatingHours = await _venueOperatingHoursRepository.GetVenueOperatingHoursByVenueIdAsync(venueId);

        //     if (venueConfiguration == null || operatingHours == null)
        //     {
        //         return new Response<List<VenueSlots>>("Configuration or operating hours not found for the specified venue")
        //         {
        //             Succeeded = false,
        //             StatusCode = HttpStatusCode.NotFound
        //         };
        //     }

        //     var (isWorking, openTime, closeTime) = GetOperatingHoursForDay(operatingHours, date.DayOfWeek);

        //     if (!isWorking || !openTime.HasValue || !closeTime.HasValue)
        //     {
        //         return new Response<List<VenueSlots>>(new List<VenueSlots>(), "The venue is closed on the given day.")
        //         {
        //             Succeeded = true,
        //             StatusCode = HttpStatusCode.OK
        //         };
        //     }

        //     var slots = GenerateSlotsForDay(venueConfiguration, openTime.Value, closeTime.Value, venueId);
        //     return new Response<List<VenueSlots>>(slots, "Slots generated successfully.")
        //     {
        //         Succeeded = true,
        //         StatusCode = HttpStatusCode.OK
        //     };
        // }

        // private (bool isWorking, TimeOnly? openTime, TimeOnly? closeTime) GetOperatingHoursForDay(VenueOperatingHours operatingHours, DayOfWeek dayOfWeek)
        // {
        //     return dayOfWeek switch
        //     {
        //         DayOfWeek.Sunday => (operatingHours.sundayIsWorking.GetValueOrDefault(), operatingHours.sundayOpenAt, operatingHours.sundayCloseAt),
        //         DayOfWeek.Monday => (operatingHours.mondayIsWorking.GetValueOrDefault(), operatingHours.mondayOpenAt, operatingHours.mondayCloseAt),
        //         DayOfWeek.Tuesday => (operatingHours.tuesdayIsWorking.GetValueOrDefault(), operatingHours.tuesdayOpenAt, operatingHours.tuesdayCloseAt),
        //         DayOfWeek.Wednesday => (operatingHours.wednesdayIsWorking.GetValueOrDefault(), operatingHours.wednesdayOpenAt, operatingHours.wednesdayCloseAt),
        //         DayOfWeek.Thursday => (operatingHours.thursdayIsWorking.GetValueOrDefault(), operatingHours.thursdayOpenAt, operatingHours.thursdayCloseAt),
        //         DayOfWeek.Friday => (operatingHours.fridayIsWorking.GetValueOrDefault(), operatingHours.fridayOpenAt, operatingHours.fridayCloseAt),
        //         DayOfWeek.Saturday => (operatingHours.saturdayIsWorking.GetValueOrDefault(), operatingHours.saturdayOpenAt, operatingHours.saturdayCloseAt),
        //         _ => (false, null, null)
        //     };
        // }

        // private List<VenueSlots> GenerateSlotsForDay(NewVenueBookingConfiguration config, TimeOnly openTime, TimeOnly closeTime, int venueId)
        // {
        //     var slots = new List<VenueSlots>();
        //     if (config.SlotTime <= 0)
        //     {
        //         throw new ArgumentException("Slot time must be a positive value.");
        //     }

        //     TimeOnly currentTime = openTime;

        //     while (currentTime.AddMinutes(config.SlotTime) <= closeTime)
        //     {
        //         var endTime = currentTime.AddMinutes(config.SlotTime);
        //         slots.Add(new VenueSlots
        //         {
        //             VenueId = venueId,
        //             StartTime = currentTime,
        //             EndTime = endTime
        //         });

        //         currentTime = endTime;
        //     }

        //     return slots;
        // }
        public async Task<Response<bool>> DeleteVenueBookingWithSlotsByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<VenueBooking>().GetWithIncludesAsync(
                    id,
                    e => e.BookingSlots,
                    e => e.Venue
                );

                if (entity is null)
                {
                    return new ResponseHandler().NotFound<bool>("VenueBooking is not found.");
                }

                var venueConfiguration = await _venueConfigurationRepository.GetVenueConfigurationByVenueID(entity.VenueId);

                var earliestBookingDateTime = entity.BookingDate.Date + entity.BookingSlots.Min(s => s.SlotStart).ToTimeSpan();
                var currentDateTime = DateTime.Now;
                var cancellationCutoffDateTime = earliestBookingDateTime.AddMinutes(-venueConfiguration.CancellationTime);

                if (currentDateTime > cancellationCutoffDateTime)
                {
                    return new ResponseHandler().BadRequest<bool>("Cancellation is not allowed within the venue's cancellation period.");
                }

                _unitOfWork.Repository<VenueBooking>().SoftDelete(entity);
                foreach (var slot in entity.BookingSlots)
                {
                    slot.IsDeleted = true;
                }

                await _unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<bool>> EditEventBookingWithSlotsAsync(EditEventBookingDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<VenueBooking>().GetWithIncludesAsync(dto.Id, e => e.BookingSlots);

                if (entity is null)
                    return new ResponseHandler().NotFound<bool>("Out of Service Book Venue is not found.");

                entity.VenueId = dto.VenueId;
                entity.BookingDate = dto.BookingDate;
                entity.BookingType = dto.BookingTypeID;
                entity.EventName = dto.EventName;
                entity.Price = dto.Price;
                entity.EventypeID = dto.EventypeID;
                entity.Description = dto.Description;
                entity.IsNonMember = dto.IsNonMember;

                foreach (var slot in entity.BookingSlots)
                {
                    slot.IsDeleted = true;
                }

                foreach (var slot in dto.TimeSlots)
                {
                    var venueBookingSlot = new VenueBookingSlot
                    {
                        SlotStart = slot.SlotStart,
                        SlotEnd = slot.SlotEnd,
                        VenueBookingId = dto.Id
                    };
                    entity.BookingSlots.Add(venueBookingSlot);
                }

                _unitOfWork.Repository<VenueBooking>().Update(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().NotFound<bool>("Something went wrong");
            }
        }

        public async Task<Response<bool>> EditEventMemberBookingWithSlotsAsync(EditEventMemberBookingDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<VenueBooking>().GetWithIncludesAsync(dto.Id, e => e.BookingSlots);

                if (entity is null)
                    return new ResponseHandler().NotFound<bool>("Out of Service Book Venue is not found.");

                entity.VenueId = dto.VenueId;
                entity.BookingDate = dto.BookingDate;
                entity.BookingType = dto.BookingTypeID;
                entity.MemberID = dto.MemberID;
                entity.Price = dto.Price;
                entity.MemberName = dto.MemberName;
                entity.PhoneNumber = dto.PhoneNumber;

                foreach (var slot in entity.BookingSlots)
                {
                    slot.IsDeleted = true;
                }

                foreach (var slot in dto.TimeSlots)
                {
                    var venueBookingSlot = new VenueBookingSlot
                    {
                        SlotStart = slot.SlotStart,
                        SlotEnd = slot.SlotEnd,
                        VenueBookingId = dto.Id
                    };
                    entity.BookingSlots.Add(venueBookingSlot);
                }

                _unitOfWork.Repository<VenueBooking>().Update(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().NotFound<bool>("Something went wrong");
            }
        }

        public async Task<Response<bool>> EditOutOfServiceEventBookingWithSlotsAsync(BaseEditBookingDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<VenueBooking>().GetWithIncludesAsync(dto.Id, e => e.BookingSlots);

                if (entity is null)
                    return new ResponseHandler().NotFound<bool>("Out of Service Book Venue is not found.");

                entity.VenueId = dto.VenueId;
                entity.BookingDate = dto.BookingDate;
                entity.BookingType = dto.BookingTypeID;

                foreach (var slot in entity.BookingSlots)
                {
                    slot.IsDeleted = true;
                }

                foreach (var slot in dto.TimeSlots)
                {
                    var venueBookingSlot = new VenueBookingSlot
                    {
                        SlotStart = slot.SlotStart,
                        SlotEnd = slot.SlotEnd,
                        VenueBookingId = dto.Id
                    };
                    entity.BookingSlots.Add(venueBookingSlot);
                }

                _unitOfWork.Repository<VenueBooking>().Update(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().NotFound<bool>("Something went wrong");
            }
        }

        public async Task<Response<GetRescheduleMemberDetails>> GetRescheduleMemberBookingDetails(int bookingID)
        {
            try
            {
                var bookingDetails = await _venueRepository.GetRescheduleMemberDetails(bookingID);

                if (bookingDetails is null)
                {
                    return new ResponseHandler().NotFound<GetRescheduleMemberDetails>("Booking details not found");
                }

                bookingDetails.VenueImage = await DocumentSetting.GetPreSignedUrlAsync(bookingDetails.VenueImage);

                return new ResponseHandler().Success(bookingDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetRescheduleMemberDetails>("Something went wrong");
            }
        }
        public async Task<Response<GetRescheduleOutOfServiceDetails>> GetRescheduleOutOfServiceBookingDetails(int bookingID)
        {
            try
            {
                var bookingDetails = await _venueRepository.GetRescheduleOutOfServiceBookingDetails(bookingID);

                if (bookingDetails is null)
                {
                    return new ResponseHandler().NotFound<GetRescheduleOutOfServiceDetails>("Booking details not found");
                }

                bookingDetails.VenueImage = await DocumentSetting.GetPreSignedUrlAsync(bookingDetails.VenueImage);

                return new ResponseHandler().Success(bookingDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetRescheduleOutOfServiceDetails>("Something went wrong");
            }
        }
        public async Task<Response<GetRescheduleEventDetails>> GetRescheduleEventBookingDetails(int bookingID)
        {
            try
            {
                var bookingDetails = await _venueRepository.GetRescheduleEventBookingDetails(bookingID);

                if (bookingDetails is null)
                {
                    return new ResponseHandler().NotFound<GetRescheduleEventDetails>("Booking details not found");
                }

                bookingDetails.VenueImage = await DocumentSetting.GetPreSignedUrlAsync(bookingDetails.VenueImage);

                return new ResponseHandler().Success(bookingDetails);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetRescheduleEventDetails>("Something went wrong");
            }
        }

        public async Task<Response<bool>> EditBookingSlotsAsync(GeneralEditDto dto)
        {
            try
            {
                var entity = await _venueBookingRepository.GetWithIncludesAsync(dto.BookingID, vb => vb.BookingSlots);

                if (entity is null)
                    return new ResponseHandler().NotFound<bool>("Booked Venue is not found.");


                if (dto.BookingDate is not null)
                {
                    entity.BookingDate = dto.BookingDate ?? DateTime.MinValue;
                }

                foreach (var slot in entity.BookingSlots)
                {
                    slot.IsDeleted = true;
                }

                foreach (var slot in dto.TimeSlots)
                {
                    var venueBookingSlot = new VenueBookingSlot
                    {
                        SlotStart = TimeOnly.Parse(slot.SlotStart),
                        SlotEnd = TimeOnly.Parse(slot.SlotEnd),
                        VenueBookingId = dto.BookingID
                    };
                    entity.BookingSlots.Add(venueBookingSlot);
                }

                _unitOfWork.Repository<VenueBooking>().Update(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().NotFound<bool>("Something went wrong");
            }
        }
        public async Task<Response<VenueUsageResponseDTO>> GetVenuesUsage(VenueUsageRequestModel model)
        {
            try
            {
                var UsageObject = await _venueBookingRepository.GetVenueUsageAsync(model);

                return new ResponseHandler().Success(UsageObject);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().NotFound<VenueUsageResponseDTO>("Something went wrong");
            }
        }

        #endregion

        #region swimming venue validation
        public async Task<Response<string>> ValidateVenueReccuringEventOperatingHoursAsync(List<RecurringEventDto> events)
        {
            try
            {
                if (events == null || events.Count == 0)
                    return new ResponseHandler().BadRequest<string>("No events provided");

                var venueId = events.FirstOrDefault().VenueId;

                var spec = new GeneralSpecifications<NewVenue>(v => v.Id == venueId)
                {
                    Includes =
                    [
                        v => v.Include(x => x.OperatingHours)
                    ]
                };

                var venue = await _unitOfWork.Repository<NewVenue>().GetWithSpecifications(spec);

                if (venue == null)
                    return new ResponseHandler().NotFound<string>($"Venue with ID {venueId} not found");

                if (venue.AllowMultipleEventsAtSameTime == true)
                    return new ResponseHandler().Success("Venue allows multiple events at the same time; conflict validation is not required");

                if (venue.OperatingHours == null)
                    return new ResponseHandler().BadRequest<string>($"Venue '{venue.Name}' does not have operating hours set");

                //  Validate each event
                foreach (var ev in events)
                {
                    if (!ev.EventDate.HasValue)
                        return new ResponseHandler().BadRequest<string>($"Event '{ev.EventName}' does not have a date");

                    var dayOfWeek = ev.EventDate.Value.DayOfWeek;

                    TimeOnly? open = null;
                    TimeOnly? close = null;
                    bool isWorking = false;

                    // Pick the correct day's schedule
                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            open = venue.OperatingHours.sundayOpenAt;
                            close = venue.OperatingHours.sundayCloseAt;
                            isWorking = venue.OperatingHours.sundayIsWorking ?? false;
                            break;
                        case DayOfWeek.Monday:
                            open = venue.OperatingHours.mondayOpenAt;
                            close = venue.OperatingHours.mondayCloseAt;
                            isWorking = venue.OperatingHours.mondayIsWorking ?? false;
                            break;
                        case DayOfWeek.Tuesday:
                            open = venue.OperatingHours.tuesdayOpenAt;
                            close = venue.OperatingHours.tuesdayCloseAt;
                            isWorking = venue.OperatingHours.tuesdayIsWorking ?? false;
                            break;
                        case DayOfWeek.Wednesday:
                            open = venue.OperatingHours.wednesdayOpenAt;
                            close = venue.OperatingHours.wednesdayCloseAt;
                            isWorking = venue.OperatingHours.wednesdayIsWorking ?? false;
                            break;
                        case DayOfWeek.Thursday:
                            open = venue.OperatingHours.thursdayOpenAt;
                            close = venue.OperatingHours.thursdayCloseAt;
                            isWorking = venue.OperatingHours.thursdayIsWorking ?? false;
                            break;
                        case DayOfWeek.Friday:
                            open = venue.OperatingHours.fridayOpenAt;
                            close = venue.OperatingHours.fridayCloseAt;
                            isWorking = venue.OperatingHours.fridayIsWorking ?? false;
                            break;
                        case DayOfWeek.Saturday:
                            open = venue.OperatingHours.saturdayOpenAt;
                            close = venue.OperatingHours.saturdayCloseAt;
                            isWorking = venue.OperatingHours.saturdayIsWorking ?? false;
                            break;
                    }

                    // Validations
                    if (!isWorking)
                        return new ResponseHandler().BadRequest<string>($"Venue '{venue.Name}' is closed on {dayOfWeek}");

                    if (!open.HasValue || !close.HasValue)
                        return new ResponseHandler().BadRequest<string>($"Operating hours for {dayOfWeek} are not set at '{venue.Name}'");

                    if (ev.From < open.Value || ev.To > close.Value)
                        return new ResponseHandler().BadRequest<string>(
                            $"Event '{ev.EventName}' on {dayOfWeek} is outside venue operating hours ({open} - {close})"
                        );
                }

                return new ResponseHandler().Success("All events are within venue operating hours");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>("something want Wrong");
            }
        }
        public async Task<Response<string>> ValidateVenueEventOperatingHoursAsync(AddEventWithVenueDto ev)
        {
            try
            {
                if (ev == null)
                    return new ResponseHandler().BadRequest<string>("No event provided");

                var venueId = ev.VenueId;

                var spec = new GeneralSpecifications<NewVenue>(v => v.Id == venueId)
                {
                    Includes =
                    [
                        v => v.Include(x => x.OperatingHours)
                    ]
                };

                var venue = await _unitOfWork.Repository<NewVenue>().GetWithSpecifications(spec);

                if (venue == null)
                    return new ResponseHandler().NotFound<string>($"Venue with ID {venueId} not found");

                if (venue.AllowMultipleEventsAtSameTime == true)
                    return new ResponseHandler().Success("Venue allows multiple events at the same time; conflict validation is not required");

                if (venue.OperatingHours == null)
                    return new ResponseHandler().BadRequest<string>($"Venue '{venue.Name}' does not have operating hours set");

                var dayOfWeek = ev.EventDate.DayOfWeek;

                TimeOnly? open = null;
                TimeOnly? close = null;
                bool isWorking = false;

                // Pick the correct day's schedule
                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        open = venue.OperatingHours.sundayOpenAt;
                        close = venue.OperatingHours.sundayCloseAt;
                        isWorking = venue.OperatingHours.sundayIsWorking ?? false;
                        break;
                    case DayOfWeek.Monday:
                        open = venue.OperatingHours.mondayOpenAt;
                        close = venue.OperatingHours.mondayCloseAt;
                        isWorking = venue.OperatingHours.mondayIsWorking ?? false;
                        break;
                    case DayOfWeek.Tuesday:
                        open = venue.OperatingHours.tuesdayOpenAt;
                        close = venue.OperatingHours.tuesdayCloseAt;
                        isWorking = venue.OperatingHours.tuesdayIsWorking ?? false;
                        break;
                    case DayOfWeek.Wednesday:
                        open = venue.OperatingHours.wednesdayOpenAt;
                        close = venue.OperatingHours.wednesdayCloseAt;
                        isWorking = venue.OperatingHours.wednesdayIsWorking ?? false;
                        break;
                    case DayOfWeek.Thursday:
                        open = venue.OperatingHours.thursdayOpenAt;
                        close = venue.OperatingHours.thursdayCloseAt;
                        isWorking = venue.OperatingHours.thursdayIsWorking ?? false;
                        break;
                    case DayOfWeek.Friday:
                        open = venue.OperatingHours.fridayOpenAt;
                        close = venue.OperatingHours.fridayCloseAt;
                        isWorking = venue.OperatingHours.fridayIsWorking ?? false;
                        break;
                    case DayOfWeek.Saturday:
                        open = venue.OperatingHours.saturdayOpenAt;
                        close = venue.OperatingHours.saturdayCloseAt;
                        isWorking = venue.OperatingHours.saturdayIsWorking ?? false;
                        break;
                }

                // Validations
                if (!isWorking)
                    return new ResponseHandler().BadRequest<string>($"Venue '{venue.Name}' is closed on {dayOfWeek}");

                if (!open.HasValue || !close.HasValue)
                    return new ResponseHandler().BadRequest<string>($"Operating hours for {dayOfWeek} are not set at '{venue.Name}'");

                if (ev.From < open.Value || ev.To > close.Value)
                    return new ResponseHandler().BadRequest<string>(
                        $"Event '{ev.EventName}' on {dayOfWeek} is outside venue operating hours ({open} - {close})"
                    );

                return new ResponseHandler().Success("Event is within venue operating hours");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>("Something went wrong");
            }
        }

        public async Task<Response<PaginationToReturnDto<VenueLookupDto>>> GetAllVenuesByFacility(int facilityId, VenueLookupSearchDto dto)
        {
            try
            {
                if (facilityId <= 0)
                {
                    Log.Error("Invalid FacilityId: {FacilityId}", facilityId);
                    return new ResponseHandler()
                        .BadRequest<PaginationToReturnDto<VenueLookupDto>>("Invalid FacilityId.");
                }

                // Criteria
                Expression<Func<NewVenue, bool>> criteria = v =>
                    v.FacilityId == facilityId &&
                    (string.IsNullOrEmpty(dto.SearchTerm) ||
                     v.Name.Trim().ToLower().Contains(dto.SearchTerm.Trim().ToLower()));

                // Selector
                Expression<Func<NewVenue, VenueLookupDto>> selector = v => new VenueLookupDto
                {
                    Id = v.Id,
                    Name = v.Name
                };

                // Count
                var totalCount = await _unitOfWork
                    .Repository<NewVenue>()
                    .GetCountByCriteriaAsync(criteria);

                // Specification
                var spec = new GeneralSpecifications<NewVenue>(
                    criteria,
                    dto.PageIndex,
                    dto.PageSize
                );

                spec.SetOrderByDescending(v => v.Id);
                spec.AsNoTracking = true;

                // Data
                var data = await _unitOfWork
                    .Repository<NewVenue>()
                    .GetAllWithSpecificationsAndProjection(spec, selector);

                Log.Information(
                    "Fetched {Count} venues for FacilityId {FacilityId} with search '{SearchTerm}'",
                    data.Count(),
                    facilityId,
                    dto.SearchTerm
                );

                int totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

                var result = new PaginationToReturnDto<VenueLookupDto>(
                    totalCount,
                    totalPages,
                    dto.PageSize,
                    dto.PageIndex,
                    data
                );

                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while fetching venues for FacilityId {FacilityId}", facilityId);
                return new ResponseHandler()
                    .BadRequest<PaginationToReturnDto<VenueLookupDto>>("Something went wrong.");
            }
        }

        #endregion
    }
}
