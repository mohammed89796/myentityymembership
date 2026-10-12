namespace MyEntityMembershipService.DTOs.Entity.MobileDtos
{
    public class AddCommentDto
    {
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
    }
}
