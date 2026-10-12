using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public class HightlightToReturnDto
    {
        public int Id { get; set; }

        public string PostTitle { get; set; }

        public string Description { get; set; }

        public List<string>? PostPictures { get; set; } = new List<string>();

        public DateOnly? PublishDate { get; set; }

        public int TenantId { get; set; }

        public string CreatedBy { get; set; }

        public PostType PostType { get; set; }
        public bool IsDraft { get; set; }
        public bool HasComments { get; set; }
        public bool HasLikes { get; set; }
        public bool HasShares { get; set; }
    }
}
