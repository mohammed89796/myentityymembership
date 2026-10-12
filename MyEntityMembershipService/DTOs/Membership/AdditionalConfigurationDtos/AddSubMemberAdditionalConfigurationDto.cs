namespace MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos
{
    public class AddSubMemberAdditionalConfigurationDto
    {
        public int? AdditionalConfigurationId { get; set; }
        public int SubMemberId { get; set; }
        public double Price { get; set; }
        public int UnitId { get; set; }
    }
}
