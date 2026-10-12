using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class NewVenueDiscountTypes : BaseEntity
    {
        public string TypeName { get; set; }
        public ICollection<NewVenuePricingRules>? NewVenuePricingRules { get; set; }
    }
}
