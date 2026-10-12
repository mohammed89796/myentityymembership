using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;




using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueBookingSlotRepo
{
    public class VenueBookingSlotRepository : GenericRepository<VenueBookingSlot>, IVenueBookingSlotRepository
    {
        private readonly EntityMembershipDbContext _context;

        public VenueBookingSlotRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> AnyBookingOverlapAsync(int venueId, DateTime bookingDate, TimeOnly slotStart, TimeOnly slotEnd)
        {
            var bookingDateOnly = bookingDate.Date;  // Get the date part of bookingDate

            return await _context.VenueBookingSlot
                .Where(slot => slot.VenueBooking.BookingDate.Date == bookingDateOnly  // Compare only the date part
                               && slot.VenueBooking.VenueId == venueId
                               && (slotStart >= slot.SlotStart && slotStart < slot.SlotEnd ||
                                    slotEnd > slot.SlotStart && slotEnd <= slot.SlotEnd ||
                                    slotStart <= slot.SlotStart && slotEnd >= slot.SlotEnd))
                .AnyAsync();
        }

    }
}
