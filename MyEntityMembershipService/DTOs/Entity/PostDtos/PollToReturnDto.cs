using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public class PollToReturnDto
    {
        public int Id { get; set; }

        public string PostTitle { get; set; }

        public DateOnly? PublishDate { get; set; }

        public int TenantId { get; set; }

        public string CreatedBy { get; set; }

        public string Question { get; set; }
        public List<string> Choices { get; set; } = new List<string>();

        public PostType PostType { get; set; }
        public bool IsDraft { get; set; }
        public bool HasMultiSelect { get; set; }
    }
}
