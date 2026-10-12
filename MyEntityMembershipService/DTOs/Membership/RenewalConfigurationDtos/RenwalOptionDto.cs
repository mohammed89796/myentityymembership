

namespace MyEntityMembershipService.DTOs.Membership.RenewalConfigurationDtos
{
    public class RenwalOptionDto
    {
        public int? Id { get; set; }
        public int Duration { get; set; }
        public int DurationUnitId { get; set; }

        public double Price { get; set; }
        public int PriceUnitId { get; set; }
    }
}