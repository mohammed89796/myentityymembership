


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.VenueBookingSlotRepo
{
    public interface IVenueBookingSlotRepository : IGenericRepository<VenueBookingSlot>
    {
        Task<bool> AnyBookingOverlapAsync(int venueId, DateTime bookingDate, TimeOnly slotStart, TimeOnly slotEnd);
    }
}
