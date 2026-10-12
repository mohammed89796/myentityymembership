using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Product:BaseEntity
    {

        public int ProductId { get; set; }
        public ProductType ProductType { get; set; }

        public string Name { get; set; } = null!;
        public string? Image { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ICollection<PackageProduct> PackageProducts { get; set; } = [];
        public ICollection<MembershipRequestProduct> MembershipRequestProducts { get; set; } = [];
        public ICollection<MembershipProduct> MembershipProducts { get; set; } = [];
    }
}
