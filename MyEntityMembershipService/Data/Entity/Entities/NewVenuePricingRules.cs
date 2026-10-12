using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenuePricingRules : BaseEntity
    {
        public DayOfWeek? Day { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public int CustomPrice { get; set; }
        public int VenueBookingConfigurationID { get; set; }

        [ForeignKey("VenueBookingConfigurationID")]
        [JsonIgnore] 
        public NewVenueBookingConfiguration? VenueBookingConfiguration { get; set; }
    }
}
