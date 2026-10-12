using MyEntityMembershipService.DTOs.Entity.PostTagDtos;

namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public class PostToReturnDto
    {
        public int Id { get; set; }

        public string PostTitle { get; set; }

        public string Description { get; set; }

        public string? PostPictureUrl { get; set; }

        public DateOnly PublishDate { get; set; }

        public int TenantId { get; set; }

        public string TenantName { get; set; }

        public List<PostTagToRetunDto>? PostTags { get; set; } = new List<PostTagToRetunDto>();

    }
}
