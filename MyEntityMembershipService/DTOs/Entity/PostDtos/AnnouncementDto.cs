using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public class AnnouncementDto : IPostDto
    {
        public string PostTitle { get; set; }

        public string? Description { get; set; }

        public List<IFormFile>? PostPictures { get; set; } = new List<IFormFile>();

        public DateOnly? PublishDate { get; set; }

        public int TenantId { get; set; }

        public string CreatedBy { get; set; }

        public Priority Priority { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDraft { get; set; }
    }
}
