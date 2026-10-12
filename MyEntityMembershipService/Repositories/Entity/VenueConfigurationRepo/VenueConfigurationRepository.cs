using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Configurations;
using MyEntityMembershipService.Data;



using MyEntityMembershipService.DTOs.Entity.VenueDtos;

using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueConfigurationRepo
{
    public class VenueConfigurationRepository : GenericRepository<NewVenueBookingConfiguration>, IVenueConfigurationRepository
    {
        private readonly EntityMembershipDbContext _context;

        public VenueConfigurationRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<GetVenueConfiguration> GetVenueConfigurationByVenueID(int venueId)
        {
            var venueConfiguration = await _context.NewVenueBookingConfiguration
                .Include(vc => vc.PricingRules)
                .Where(vc => vc.VenueID == venueId)
                .FirstOrDefaultAsync();

            if (venueConfiguration == null)
            {
                return null;
            }

            return new GetVenueConfiguration
            {
                SlotTime = venueConfiguration.SlotTime,
                PricePerSlot = venueConfiguration.PricePerSlot,
                MinSlotsBooked = venueConfiguration.MinSlotsBooked,
                MaxSlotsBooked = venueConfiguration.MaxSlotsBooked,
                CancellationTime = venueConfiguration.CancellationTime,
                BookingAvailability = venueConfiguration.BookingAvailability,
                BookingAvailabilityUnit = venueConfiguration.BookingAvailabilityUnit,
                IsVisible = venueConfiguration.IsVisible,
                AllowBooking = venueConfiguration.AllowBooking,
                PricingRules = venueConfiguration.PricingRules?.Select(pr => new PricingRulesDto
                {
                    Day = pr.Day,
                    StartTime = pr.StartTime,
                    EndTime = pr.EndTime,
                    CustomPrice = pr.CustomPrice
                }).ToList()
            };
        }


        public async Task<NewVenuePricingRules> GetPricingRulesByVenueIdAsync(int venueId)
        {
            return await _context.NewVenuePricingRules.Where(vc => vc.VenueBookingConfiguration.VenueID == venueId).FirstOrDefaultAsync();
        }

        public async Task<bool> AnyBookingOverlapAsync(int venueId, DateTime bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            return await _context.VenueBookingSlot
                .Include(vbs => vbs.VenueBooking)
                .AnyAsync(vbs => vbs.VenueBooking.VenueId == venueId
                                 && vbs.VenueBooking.BookingDate.Date == bookingDate.Date
                                 && (
                                     vbs.SlotStart < endTime && vbs.SlotEnd > startTime
                                     || vbs.SlotStart >= startTime && vbs.SlotStart < endTime
                                 ));
        }

        public async Task<List<NewVenuePricingRules>> GetPricingRulesByVenueIdAndDayAsync(int venueId, DayOfWeek day)
        {
            return await _context.NewVenuePricingRules
                .Where(pr => pr.VenueBookingConfiguration.VenueID == venueId && pr.Day == day)
                .ToListAsync();
        }

        public async Task<decimal?> GetPriceForSlotAsync(int venueId, DayOfWeek day, TimeOnly slotStart, TimeOnly slotEnd)
        {
            var rule = await _context.NewVenuePricingRules
                .FirstOrDefaultAsync(pr => pr.VenueBookingConfiguration.VenueID == venueId && pr.Day == day
                                           && pr.StartTime <= slotStart
                                           && pr.EndTime >= slotEnd);
            return rule?.CustomPrice;
        }

    }
}
