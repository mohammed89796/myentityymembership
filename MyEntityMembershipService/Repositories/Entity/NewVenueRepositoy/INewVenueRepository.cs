
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.NewVenueRepositoy
{
    public interface INewVenueRepository : IGenericRepository<NewVenue>
    {
        Task<GetVenuePaginationListDto> GetVenuesPaginatedAsync(int pageNumber, int facilityId);
        Task<GetVenuePaginationListDto> GetMobileVenuesPaginatedAsync(int pageNumber, int facilityId);
        Task<GetVenueDetailsDto> GetVenueDetailsAsync(int venueId);
        Task<List<GetVenueDto>> GetVenuesByListOfIdsAsync(List<int> venuesIds);
        Task<List<GetVenueWrapperDto>> GetVenuesInfoByListOfIdsAsync(List<int> venuesIds);
        Task<List<NewVenue>> GetVenuesByNameAsync(string venueName);
        Task<List<VenueBookingDto>> GetVenueBookingsAsync(GetVenueBookingsDto request);
        Task<GetVenueBookingsMobileDto> GetVenuesBookingsMobile(int venueID, string userId);
        Task<string> GetCancellationPolicyMessage(int venueID);
    }
}
