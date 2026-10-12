using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;



using MyEntityMembershipService.DTOs.Entity.VenueDtos;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueBookingRepo
{
    public class VenueBookingRepository : GenericRepository<VenueBooking>, IVenueBookingRepository
    {
        private readonly EntityMembershipDbContext _context;

        public VenueBookingRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<VenueBooking> GetBookingByVenueAndDate(int venueId, DateTime date)
        {
            DateTime startOfDay = date.Date;
            DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var booking = await _context.VenueBooking
                .Where(b => b.VenueId == venueId
                            && b.BookingDate >= startOfDay
                            && b.BookingDate <= endOfDay)
                .FirstOrDefaultAsync();

            return booking;
        }
        public async Task<VenueUsageResponseDTO> GetVenueUsageAsync(VenueUsageRequestModel request)
        {
            var Interval = 50;
            var query = _context.VenueBooking
                .Include(vb => vb.BookingSlots) 
                .Include(vb => vb.Venue)
                .AsQueryable();

            if (request.BranchID.HasValue)
                query = query.Where(vb => vb.Venue.Facility.BranchId == request.BranchID.Value);
            
            if (request.FacilityID.HasValue)
                query = query.Where(vb => vb.Venue.FacilityId == request.FacilityID.Value);

            if (request.Year.HasValue)
                query = query.Where(vb => vb.BookingDate.Year == request.Year);

            if (request.Month.HasValue)
                query = query.Where(vb => vb.BookingDate.Month == request.Month);

            var bookings = await query.ToListAsync();

            var monthlyUsage = bookings
                .GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
                .Select(g => new MonthlyUsageDTO
                {
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    VenueUsage = g.GroupBy(b => b.VenueId)
                        .Select(vg => new VenueUsageDTO
                        {
                            VenueID = vg.Key,
                            VenueName = vg.First().Venue.Name,
                            Usage = (int)Math.Round(vg.Sum(b => b.BookingSlots?.Sum(slot =>
                                    (slot.SlotEnd - slot.SlotStart).TotalHours) ?? 0))
                        })
                        .ToList()
                })
                .OrderBy(m => DateTime.ParseExact(m.MonthName, "MMMM yyyy", null))
                .ToList();

            var allUsages = monthlyUsage.SelectMany(m => m.VenueUsage.Select(v => v.Usage)).ToList();
            var maxUsage = allUsages.Any() ? allUsages.Max() : 0;
            var minUsage = allUsages.Any() ? allUsages.Min() : 0;

            return new VenueUsageResponseDTO
            {
                MaxUsage = maxUsage + Interval,
                MinUsage = 0,
                Interval = Interval,
                MonthlyUsage = monthlyUsage
            };
        }


    }
}
