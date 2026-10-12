using MyEntityMembershipService.Configurations;



using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.VenueConfigurationRepo
{
    public interface IVenueConfigurationRepository : IGenericRepository<NewVenueBookingConfiguration>
    {
        Task<GetVenueConfiguration> GetVenueConfigurationByVenueID(int venueId);
        Task<NewVenuePricingRules> GetPricingRulesByVenueIdAsync(int venueId);
        Task<List<NewVenuePricingRules>> GetPricingRulesByVenueIdAndDayAsync(int venueId, DayOfWeek day);
        Task<decimal?> GetPriceForSlotAsync(int venueId, DayOfWeek day, TimeOnly slotStart, TimeOnly slotEnd);
    }
}
