namespace MyEntityMembershipService.DTOs.Entity.MobileDtos
{
    public class CommentDto
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public string CreatedBy { get; set; }
        public string UserName { get; set; }

        public int? PostId { get; set; }

        public int? ParentCommentId { get; set; }

        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();


    }
}
