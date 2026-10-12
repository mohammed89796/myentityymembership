


using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.VenueBookingRepo
{
    public interface IVenueBookingRepository : IGenericRepository<VenueBooking>
    {
        Task<VenueBooking> GetBookingByVenueAndDate(int venueId, DateTime date);
        Task<VenueUsageResponseDTO> GetVenueUsageAsync(VenueUsageRequestModel request);

    }
}
