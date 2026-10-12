using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PackageProduct:BaseEntity
    {
        public int PackageId { get; set; }
        public Package Package { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }


        // Common Limits (fast queries)
        public int? MaxSessions { get; set; }
        public SessionInterval? SessionInterval { get; set; }
        public int? PerInterval { get; set; }



    }
}
