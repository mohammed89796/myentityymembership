namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public class HighlightDto : IPostDto
    {
        public string PostTitle { get; set; }

        public string? Description { get; set; }

        public List<IFormFile>? PostPictures { get; set; } = new List<IFormFile>();

        public DateOnly? PublishDate { get; set; }

        public int TenantId { get; set; }

        public string CreatedBy { get; set; }

        public bool IsDraft { get; set; }
        public bool HasComments { get; set; }
        public bool HasLikes { get; set; }
        public bool HasShares { get; set; }
    }
}
