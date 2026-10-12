using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class OrganizationEntity : BaseEntity
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string? logo { get; set; }
        public string? websiteUrl { get; set; }
        public string? description { get; set; }
        public ICollection<Facility>? Facilities { get; set; }
    }
}
