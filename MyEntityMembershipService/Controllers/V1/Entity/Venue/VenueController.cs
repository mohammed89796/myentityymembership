using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs.Entity.EventDtos;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Entity.VenueService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Venue
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class VenueController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EntityMembershipDbContext _Context;
        private readonly IConfiguration configuration;
        private readonly IVenueService _venueService;
        private readonly IUnitOfWork _unitOfWork;
        public VenueController(IUnitOfWork unitOfWork, IMapper mapper, EntityMembershipDbContext Context, IConfiguration configuration, IVenueService venueService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _Context = Context;
            this.configuration = configuration;
            _venueService = venueService;
        }

        [HttpGet("GetAllVenues")]
        public async Task<IResult> GetAllVenues([FromQuery]VenueQueryPrametersDto queryPrameters)
        {
            var response = await _venueService.GetAllVenues(queryPrameters);
            return response.GetResponse();
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var Venues = _Context.Venues.Include(v => v.VenueAccessibilityFeatures).Include(v => v.VenueStadFacilities).Include(v => v.Facility).ThenInclude(f => f.FacilityOperatingHours).Include(v => v.VenueOperatingHours).ToList();
              
                var tasks= Venues.Select(async venue =>
                {
                    venue.Image = await DocumentSetting.GetPreSignedUrlAsync( venue.Image);
                });

                await Task.WhenAll(tasks);
                return Ok(Venues);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet("Get-AllAccessibilityFeatures")]
        public async Task<IActionResult> GetAllAccessibilityFeatures()
        {
            try
            {
                var AccessibilityFeatures = _Context.AccessibilityFeatures.ToList();
                return Ok(AccessibilityFeatures);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet("Get-AllFacilities")]
        public async Task<IActionResult> GetAllFacilities()
        {
            try
            {
                var StadFacilities = _Context.StadFacilities.ToList();
                return Ok(StadFacilities);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet("Get-AllSportsAccommodated")]
        public async Task<IActionResult> GetAllSportsAccommodated()
        {
            try
            {
                var StadFacilities = _Context.SportsAccommodated.ToList();
                return Ok(StadFacilities);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet("Get-AllFacilityType")]
        public async Task<IActionResult> GetAllFacilityType()
        {
            try
            {
                var StadFacilities = _Context.FacilityType.ToList();
                return Ok(StadFacilities);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet("GetByPagination")]
        public async Task<IActionResult> GetByPagination(int pageNo = 1, int pageSize = 10)
        {
            try
            {
                var GetVenuePaginationDto = new GetVenuePaginationDto();
                var CountOflistOfVenues = (int)Math.Ceiling((double)_Context.Venues.Count() / pageSize);
                var GetlistOfVenues = _Context.Venues.Skip((pageNo - 1) * pageSize).Take(pageSize).Include(v => v.VenueAccessibilityFeatures).Include(v => v.VenueStadFacilities).Include(v => v.Facility).ThenInclude(f => f.FacilityOperatingHours).Include(v => v.VenueOperatingHours).ToList();
               
                var tasks = GetlistOfVenues.Select(async venue =>
                {
                    venue.Image = await DocumentSetting.GetPreSignedUrlAsync( venue.Image);
                });
                await Task.WhenAll(tasks);

                GetVenuePaginationDto.totalPages = CountOflistOfVenues;
                GetVenuePaginationDto.currentPage = pageNo;
                GetVenuePaginationDto.venues = GetlistOfVenues;
                return Ok(GetVenuePaginationDto);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpGet("GetOldVenueById/{id}")]
        public async Task<IActionResult> GetOldVenueById(int id)
        {
            try
            {
                var Venue = await _Context.Venues.Include(v => v.VenueAccessibilityFeatures).Include(v => v.VenueStadFacilities).Where(or => or.Id == id).Include(v => v.Facility).ThenInclude(f => f.FacilityOperatingHours).Include(v => v.VenueOperatingHours).FirstOrDefaultAsync();
               
                Venue.Image = await DocumentSetting.GetPreSignedUrlAsync( Venue.Image);

                return Ok(Venue);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var Venue = await _Context.NewVenue.Where(or => or.Id == id).Include(v => v.Facility).ThenInclude(f => f.FacilityOperatingHours).Include(v => v.OperatingHours).FirstOrDefaultAsync();
                
                Venue.VenueImageURL = await DocumentSetting.GetPreSignedUrlAsync( Venue.VenueImageURL);

                return Ok(Venue);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpPost("CreateVenue")]
        public async Task<IActionResult> CreateVenue(AddVenueDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_Context.Venues.Any(or => or.name == model.name))
                    {
                        return BadRequest("This Name Already Exist");
                    }
                    var Venue = _mapper.Map<Data.Entity.Entities.Venue>(model);
                    Venue.Image = await DocumentSetting.AddToContainerAndGetUrl(configuration["StorageConnectionString"], "buildupimagedocs", model.ImageUrl);

                    var venueId = await _Context.Venues.AddAsync(Venue);
                    _Context.SaveChanges();
                    foreach (var item in model.accessibilityFeaturesId)
                    {
                        var VenueAccessibilityFeatures = new VenueAccessibilityFeatures
                        {
                            VenueId = venueId.Entity.Id,
                            AccessibilityFeaturesId = item
                        };
                        await _unitOfWork.Repository<VenueAccessibilityFeatures>().Add(VenueAccessibilityFeatures);
                    }

                    foreach (var item in model.VenueStadFacilitiesId)
                    {
                        var VenueStadFacility = new VenueStadFacility
                        {
                            VenueId = venueId.Entity.Id,
                            StadFacilityId = item
                        };
                        await _unitOfWork.Repository<VenueStadFacility>().Add(VenueStadFacility);
                    }
                    await _unitOfWork.Save();
                    return Ok(venueId.Entity.Id);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpPut("UpdateVenue")]
        public async Task<IActionResult> UpdateVenue(UpdateVenueDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Venue = await _Context.Venues.Include(v => v.VenueAccessibilityFeatures).Include(v => v.VenueStadFacilities).Include(v => v.VenueOperatingHours).Where(or => or.Id == model.Id).FirstOrDefaultAsync();
                    if (Venue is null)
                    {
                        return BadRequest("This Venue Not Found");
                    }
                    Venue.name = model.name;
                    Venue.matchFormats = model.matchFormats;
                    Venue.typeOfCourtSurface = model.typeOfCourtSurface;
                    Venue.typeOfTurf = model.typeOfTurf;
                    Venue.sportAccommodated = model.sportAccommodated;
                    Venue.basketHeight = model.basketHeight;
                    Venue.courtDimensions = model.courtDimensions;
                    Venue.fieldDimensions = model.fieldDimensions;
                    Venue.goalSize = model.goalSize;
                    Venue.playerAndOfficialFacilities = model.playerAndOfficialFacilities;
                    Venue.playerFacilities = model.playerFacilities;
                    Venue.seatingAvailability = model.seatingAvailability;
                    Venue.venueLocation = model.venueLocation;
                    await _Context.SaveChangesAsync();
                    _unitOfWork.Repository<VenueAccessibilityFeatures>().RemoveRange(Venue.VenueAccessibilityFeatures);
                    _unitOfWork.Repository<VenueStadFacility>().RemoveRange(Venue.VenueStadFacilities);

                    foreach (var item in model.accessibilityFeaturesId)
                    {
                        var VenueAccessibilityFeatures = new VenueAccessibilityFeatures
                        {
                            VenueId = model.Id,
                            AccessibilityFeaturesId = item
                        };
                        await _unitOfWork.Repository<VenueAccessibilityFeatures>().Add(VenueAccessibilityFeatures);
                    }

                    foreach (var item in model.VenueStadFacilitiesId)
                    {
                        var VenueStadFacility = new VenueStadFacility
                        {
                            VenueId = model.Id,
                            StadFacilityId = item
                        };
                        await _unitOfWork.Repository<VenueStadFacility>().Add(VenueStadFacility);
                    }
                    await _unitOfWork.Save();
                    return Ok("Venue Updated Successfully");
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Venue = await _Context.Venues.Include(v => v.VenueAccessibilityFeatures).Include(v => v.VenueStadFacilities).Include(v => v.Facility).ThenInclude(f => f.FacilityOperatingHours).Include(v => v.VenueOperatingHours).Where(or => or.Id == id).FirstOrDefaultAsync();
                if (Venue is null)
                {
                    return BadRequest("Venue Not Found");
                }
                _unitOfWork.Repository<Data.Entity.Entities.Venue>().Delete(Venue);
                _unitOfWork.Repository<VenueAccessibilityFeatures>().RemoveRange(Venue.VenueAccessibilityFeatures);
                _unitOfWork.Repository<VenueStadFacility>().RemoveRange(Venue.VenueStadFacilities);
                await _unitOfWork.Save();
                return Ok("Venue Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpPost("SetOpreatingHours")]
        public async Task<IActionResult> SetOpreatingHours(SetVenueOperatingDto model)
        {
            try
            {
                if (model is null)
                {
                    return BadRequest("Invaild Object");
                }
                var OpreatingHoursMapping = _mapper.Map<VenueOperatingHours>(model);
                await _unitOfWork.Repository<VenueOperatingHours>().Add(OpreatingHoursMapping);
                await _unitOfWork.Save();
                return Ok(model);
            }
            catch (Exception ex)
            {

                return BadRequest("Something went wrong");
            }

        }
        [HttpPut("UpdateOpreatingHours")]
        public async Task<IActionResult> UpdateOpreatingHours(UpdateVenueOperatingHoursDto model)
        {
            try
            {
                if (model is null)
                {
                    return BadRequest("Invaild Object");
                }
                //     var OpreatingHours = await _unitOfWork.Repository<VenueOperatingHours>().GetByIdAsync(model.Id);
                var OpreatingHoursMapping = _mapper.Map<VenueOperatingHours>(model);
                _unitOfWork.Repository<VenueOperatingHours>().Update(OpreatingHoursMapping);
                await _unitOfWork.Save();
                return Ok("Opreating Hours Updated Successfully");
            }
            catch (Exception ex)
            {

                return BadRequest("Something went wrong");
            }

        }

        [HttpPost("IsVenuesIdForSpecificTenantValid/{tenantId}")]
        public async Task<IResult> IsVenuesIdForSpecificTenantValid(int tenantId, List<int> venusId)
            => (await _venueService.IsVenueIdsValidForSpecificTenant(tenantId, venusId)).GetResponse();

        [HttpPost("GetVenuesByIds")]
        public async Task<IResult> GetVenuesByIds(List<int> venusId)
            => (await _venueService.GetVenuesByListOfIds(venusId)).GetResponse();

        [HttpPost("GetVenuesInfoByListOfIds")]
        public async Task<IResult> GetVenuesInfoByListOfIds(List<int> venusId)
            => (await _venueService.GetVenuesInfoByListOfIds(venusId)).GetResponse();

        [HttpGet("GetVenuesByName/{venueName}")]
        public async Task<IResult> GetVenuesByName(string venueName)
        {
            var result = await _venueService.GetVenuesByName(venueName);
            return result.GetResponse();
        }
        [HttpGet("GetVenueListByFacilityID")]
        public async Task<IResult> GetVenueListByFacilityID(int pageNumber, int facilityID)
        {
            var result = await _venueService.GetVenueListByFacilityID(pageNumber, facilityID);
            return result.GetResponse();
        }
        [HttpGet("GetMobileVenueListByFacilityID")]
        public async Task<IResult> GetMobileVenueListByFacilityID(int pageNumber, int facilityID)
        {
            var result = await _venueService.GetMobileVenueListByFacilityID(pageNumber, facilityID);
            return result.GetResponse();
        }
        [HttpGet("GetVenueDetailsByIdAsync/{venueID}")]
        public async Task<IResult> GetVenueDetailsByIdAsync(int venueID)
        {
            var result = await _venueService.GetVenueDetailsByIdAsync(venueID);
            return result.GetResponse();
        }

        [HttpPost("CreateNewVenue")]
        public async Task<IResult> CreateNewVenue([FromForm] AddNewVenueDto venueDto)
             => (await _venueService.CreateNewVenue(venueDto)).GetResponse();

        [HttpPost("AddVenueOperatingHoursAsync")]
        public async Task<IResult> AddVenueOperatingHoursAsync([FromForm] VenueOperatingHoursDto venueDto)
             => (await _venueService.AddVenueOperatingHoursAsync(venueDto)).GetResponse();

        [HttpDelete("RemoveVenueAsync/{venueID}")]
        public async Task<IResult> RemoveVenueAsync(int venueID)

       => (await _venueService.RemoveVenueAsync(venueID)).GetResponse();

        [HttpDelete("RemoveVenueMediaAsync")]
        public async Task<IResult> RemoveVenueMediaAsync(int venueID, int mediaId)
        {
            var result = await _venueService.RemoveVenueMediaAsync(venueID, mediaId);
            return result.GetResponse();
        }

        [HttpPost("EditVenueAsync")]
        public async Task<IResult> EditVenueAsync([FromForm] EditVenueDto venueDto)
            => (await _venueService.EditVenueAsync(venueDto)).GetResponse();
        [HttpGet("GetVenueEditDetailsByIdAsync/{venueID}")]
        public async Task<IResult> GetVenueEditDetailsByIdAsync(int venueID)
            => (await _venueService.GetVenueEditDetailsByIdAsync(venueID)).GetResponse();
        [HttpGet("GetMobileVenueDetailsAsync/{venueID}")]
        public async Task<IResult> GetMobileVenueDetailsAsync(int venueID)
            => (await _venueService.GetMobileVenueDetailsAsync(venueID)).GetResponse();
        [HttpPost("AddVenueNewMedia")]
        public async Task<IResult> AddVenueNewMedia([FromForm] List<IFormFile> newMedia, int venueID)
        {
            var result = await _venueService.AddVenueNewMedia(newMedia, venueID);
            return result.GetResponse();

        }

        [HttpPut("UpdateVenueOperatingHoursAsync")]
        public async Task<IResult> UpdateVenueOperatingHoursAsync(EditVenueOperatingHoursDto venueDto)
        {
            var result = await _venueService.UpdateVenueOperatingHoursAsync(venueDto);
            return result.GetResponse();
        }
        [HttpGet("GetVenueBookingHeader/{venueId}")]
        public async Task<IResult> GetVenueBookingHeader(int venueId)
        {
            var result = await _venueService.GetVenueBookingHeader(venueId);
            return result.GetResponse();
        }
        [HttpGet("GetVenueOperatingHours/{venueId}")]
        public async Task<IResult> GetVenueOperatingHours(int venueId)
        {
            var result = await _venueService.GetVenueOperatingHours(venueId);
            return result.GetResponse();
        }
        [HttpPost("GetAvailableSlots")]
        public async Task<IResult> GetAvailableSlots(GetAvailableSlotsRequestDto request)
        {
            var result = await _venueService.GetAvailableSlots(request);
            return result.GetResponse();
        }
        [HttpPost("BookVenueForEvent")]
        public async Task<IResult> BookVenueForEvent(EventBookingDto bookingDto)
        {
            var result = await _venueService.BookVenueForEvent(bookingDto);
            return result.GetResponse();
        }
        [HttpPost("BookVenueOutOfService")]
        public async Task<IResult> BookVenueOutOfService(BaseBookingDto bookingDto)
        {
            var result = await _venueService.BookVenueOutOfService(bookingDto);
            return result.GetResponse();
        }
        [HttpPost("BookVenueForMember")]
        public async Task<IResult> BookVenueForMember(EventMemberBookingDto bookingDto)
        {
            var result = await _venueService.BookVenueForMember(bookingDto);
            return result.GetResponse();
        }
        [HttpPost("GetVenuesBookings")]
        public async Task<IResult> GetVenuesBookings(GetVenueBookingsDto request)
        {
            var result = await _venueService.GetVenuesBookings(request);
            return result.GetResponse();
        }
        [HttpGet("GetVenuesBookingsMobile/{venueID}")]
        public async Task<IResult> GetVenuesBookingsMobile(int venueID)
        {
            var userId = User.GetUserId();
            var result = await _venueService.GetVenuesBookingsMobile(venueID,userId);
            return result.GetResponse();
        }
        [HttpGet("GetCancellationPolicyMessage/{venueID}")]
        public async Task<IResult> GetCancellationPolicyMessage(int venueID)
        {
            var result = await _venueService.GetCancellationPolicyMessage(venueID);
            return result.GetResponse();
        }

        [HttpDelete("DeleteVenueBookingWithSlots/{id:int}")]
        public async Task<IResult> DeleteVenueBookingWithSlotsByIdAsync(int id)
        {
            var result = await _venueService.DeleteVenueBookingWithSlotsByIdAsync(id);
            return result.GetResponse();
        }

        // [HttpPut("EditEventBookingWithSlots")]
        // public async Task<IResult> EditEventBookingWithSlots(EditEventBookingDto dto)
        // {
        //     var result = await _venueService.EditEventBookingWithSlotsAsync(dto);
        //     return result.GetResponse();
        // }
        
        // [HttpPut("EditEventMemberBookingWithSlots")]
        // public async Task<IResult> EditEventMemberBookingWithSlots(EditEventMemberBookingDto dto)
        // {
        //     var result = await _venueService.EditEventMemberBookingWithSlotsAsync(dto);
        //     return result.GetResponse();
        // }
        
        // [HttpPut("EditOutOfServiceEventBookingWithSlots")]
        // public async Task<IResult> EditOutOfServiceEventBookingWithSlotsAsync(BaseEditBookingDto dto)
        // {
        //     var result = await _venueService.EditOutOfServiceEventBookingWithSlotsAsync(dto);
        //     return result.GetResponse();
        // }
        [HttpGet("GetRescheduleMemberBookingDetails/{bookingID}")]
        public async Task<IResult> GetRescheduleMemberBookingDetails(int bookingID)
        {
            var result = await _venueService.GetRescheduleMemberBookingDetails(bookingID);
            return result.GetResponse();
        }
        [HttpGet("GetRescheduleOutOfServiceBookingDetails/{bookingID}")]
        public async Task<IResult> GetRescheduleOutOfServiceBookingDetails(int bookingID)
        {
            var result = await _venueService.GetRescheduleOutOfServiceBookingDetails(bookingID);
            return result.GetResponse();
        }
        [HttpGet("GetRescheduleEventBookingDetails/{bookingID}")]
        public async Task<IResult> GetRescheduleEventBookingDetails(int bookingID)
        {
            var result = await _venueService.GetRescheduleEventBookingDetails(bookingID);
            return result.GetResponse();
        }
        [HttpPut("EditBookingSlotsAsync")]
        public async Task<IResult> EditBookingSlotsAsync(GeneralEditDto dto)
        {
            var result = await _venueService.EditBookingSlotsAsync(dto);
            return result.GetResponse();
        }
        [HttpPost("GetVenuesUsage")]
        public async Task<IResult> GetVenuesUsage(VenueUsageRequestModel model)
        {
            var result = await _venueService.GetVenuesUsage(model);
            return result.GetResponse();
        }

        [HttpPost("ValidateVenueOperatingHours")]
        public async Task<IResult> ValidateVenueOperatingHours(List<RecurringEventDto> recurringEventDto)
        {
            var result = await _venueService.ValidateVenueReccuringEventOperatingHoursAsync(recurringEventDto);
            return result.GetResponse();
        }
        [HttpPost("ValidateVenueEventOperatingHours")]
        public async Task<IResult> ValidateVenueEventOperatingHours([FromForm] AddEventWithVenueDto eventWithVenueDto)
        {
            var result = await _venueService.ValidateVenueEventOperatingHoursAsync(eventWithVenueDto);
            return result.GetResponse();
        }

        [HttpGet("booking")]
        public async Task<IResult> GetVenueBooking([FromQuery] VenueBookingQueryParametersDto parameters)
            => (await _venueService.GetVenueBooking(parameters)).GetResponse();


        [HttpGet("{facilityId}")]
        public async Task<IResult> GetAllVenuesByFacility(int facilityId, [FromQuery] VenueLookupSearchDto dto)
            => (await _venueService.GetAllVenuesByFacility(facilityId, dto)).GetResponse();

    }
}
