using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class SocialMediaDto
    {
        public string Link { get; set; }

        public Platform Platform { get; set; }
    }
}
