namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public class PostDto
    {
        public string PostTitle { get; set; }

        public string Description { get; set; }

        public IFormFile? PostPicture { get; set; }

        public DateOnly PublishDate { get; set; }

        public int TenantId { get; set; }
        public string UserId { get; set; }
        public List<string>? PostTags { get; set; } = new List<string>();
    }
}
