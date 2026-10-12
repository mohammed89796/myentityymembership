
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.EventDtos;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.VenueService
{
    public interface IVenueService
    {
        Task<Response<PaginationToReturnDto<VenueCardDetailsDto>>> GetAllVenues(VenueQueryPrametersDto queryParam);
        Task<Response<PaginationToReturnDto<VenueBookingResponseDto>>> GetVenueBooking(
            VenueBookingQueryParametersDto param);
        Task<Response<List<GetVenueDto>>> GetVenuesByListOfIds(List<int> venuesIds);
        Task<Response<bool>> IsVenueIdsValidForSpecificTenant(int tenantId, List<int> venues);
        Task<Response<List<GetVenueWrapperDto>>> GetVenuesInfoByListOfIds(List<int> venuesIds);
        Task<Response<List<NewVenue>>> GetVenuesByName(string venueName);
        Task<Response<AddNewVenueDto>> CreateNewVenue(AddNewVenueDto venueDto);
        Task<Response<GetVenuePaginationListDto>> GetVenueListByFacilityID(int pageNumber, int facilityID);
        Task<Response<GetVenuePaginationListDto>> GetMobileVenueListByFacilityID(int pageNumber, int facilityID);
        Task<Response<GetVenueDetailsDto>> GetVenueDetailsByIdAsync(int venueID);
        Task<Response<bool>> AddVenueOperatingHoursAsync(VenueOperatingHoursDto operatingHoursDto);
        Task<Response<bool>> RemoveVenueAsync(int venueId);
        Task<Response<bool>> RemoveVenueMediaAsync(int venueID, int mediaId);
        Task<Response<EditVenueDto>> EditVenueAsync(EditVenueDto venueDto);
        Task<Response<EditVenueDetailsDto>> GetVenueEditDetailsByIdAsync(int venueID);
        Task<Response<GetVenueMobileDetailsDto>> GetMobileVenueDetailsAsync(int VenueID);
        Task<Response<bool>> AddVenueNewMedia(List<IFormFile> newMedia, int venueID);
        Task<Response<bool>> UpdateVenueOperatingHoursAsync(EditVenueOperatingHoursDto venueDto);
        Task<Response<GetVenueBookingHeaderDetailsDto>> GetVenueBookingHeader(int venueId);
        Task<Response<VenueOperatingHours>> GetVenueOperatingHours(int venueId);
        Task<Response<List<AvailableSlotDto>>> GetAvailableSlots(GetAvailableSlotsRequestDto request);
        Task<Response<bool>> BookVenueForEvent(EventBookingDto bookingDto);
        Task<Response<bool>> BookVenueOutOfService(BaseBookingDto bookingDto);
        Task<Response<bool>> BookVenueForMember(EventMemberBookingDto bookingDto);
        Task<Response<List<VenueBookingDto>>> GetVenuesBookings(GetVenueBookingsDto request);
        Task<Response<GetVenueBookingsMobileDto>> GetVenuesBookingsMobile(int venueID,string userId);
        Task<Response<string>> GetCancellationPolicyMessage(int venueID);
        Task<Response<bool>> DeleteVenueBookingWithSlotsByIdAsync(int id);
        // Task<Response<bool>> EditEventBookingWithSlotsAsync(EditEventBookingDto dto);
        // Task<Response<bool>> EditEventMemberBookingWithSlotsAsync(EditEventMemberBookingDto dto);
        // Task<Response<bool>> EditOutOfServiceEventBookingWithSlotsAsync(BaseEditBookingDto dto);
        Task<Response<GetRescheduleMemberDetails>> GetRescheduleMemberBookingDetails(int bookingID);
        Task<Response<GetRescheduleOutOfServiceDetails>> GetRescheduleOutOfServiceBookingDetails(int bookingID);
        Task<Response<GetRescheduleEventDetails>> GetRescheduleEventBookingDetails(int bookingID);
        Task<Response<bool>> EditBookingSlotsAsync(GeneralEditDto dto);
        Task<Response<VenueUsageResponseDTO>> GetVenuesUsage(VenueUsageRequestModel model);

        Task<Response<string>> ValidateVenueReccuringEventOperatingHoursAsync(List<RecurringEventDto> events);
        Task<Response<string>> ValidateVenueEventOperatingHoursAsync(AddEventWithVenueDto ev);
        Task<Response<PaginationToReturnDto<VenueLookupDto>>> GetAllVenuesByFacility(int  facilityId , VenueLookupSearchDto dto);
    }
}
