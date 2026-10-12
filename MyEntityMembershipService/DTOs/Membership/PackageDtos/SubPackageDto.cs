namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class SubPackageDto
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }

        public List<int> VenueIds { get; set; } = new();

        // Optional overrides
        public List<PackageProductDto>? Products { get; set; }
    }
}
