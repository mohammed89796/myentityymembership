

namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class UpdateTenantDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }

        public List<SocialMediaDto> SocialMediaLinks { get; set; } = new List<SocialMediaDto>();
    }

}
