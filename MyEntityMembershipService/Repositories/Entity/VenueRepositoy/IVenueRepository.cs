
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueRepositoy
{
    public interface IVenueRepository : IGenericRepository<Venue>
    {
        Task<List<object>> GetVenuesByListOfIdsAsync(List<int> venuesIds);

        Task<List<object>> GetVenuesInfoByListOfIdsAsync(List<int> venuesIds);
        Task<List<Venue>> GetVenuesByNameAsync(string venueName);
        Task<EditVenueDetailsDto> GetVenueEditDetailsByIdAsync(int venueID);
        Task<GetVenueMobileDetailsDto> GetMobileVenueDetailsAsync(int venueID);
        Task<GetVenueBookingHeaderDetailsDto> GetVenueBookingHeaderDetailsAsync(int venueId);
        Task<GetRescheduleMemberDetails> GetRescheduleMemberDetails(int bookingID);
        Task<GetRescheduleOutOfServiceDetails> GetRescheduleOutOfServiceBookingDetails(int bookingID);
        Task<GetRescheduleEventDetails> GetRescheduleEventBookingDetails(int bookingID);
    }
}
