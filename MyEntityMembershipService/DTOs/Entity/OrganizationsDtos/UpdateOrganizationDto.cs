using MyEntityMembershipService.Data;


namespace MyEntityMembershipService.DTOs.Entity.OrganizationsDtos
{
    public class UpdateOrganizationDto : BaseEntity
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public IFormFile? logo { get; set; }
        public string? websiteUrl { get; set; }
        public string? description { get; set; }
    }
}
