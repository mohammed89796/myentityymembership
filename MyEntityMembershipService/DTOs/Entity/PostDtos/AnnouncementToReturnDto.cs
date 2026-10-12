using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public class AnnouncementToReturnDto
    {
        public int Id { get; set; }
        public string PostTitle { get; set; }

        public string Description { get; set; }

        public List<string>? PostPictures { get; set; } = new List<string>();

        public DateOnly? PublishDate { get; set; }

        public int TenantId { get; set; }

        public string CreatedBy { get; set; }

        public PostType PostType { get; set; }

        public string Priority { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDraft { get; set; }
    }
}
