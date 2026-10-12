namespace MyEntityMembershipService.DTOs.Entity.MobileDtos
{
    public class AddReplyDto
    {
        public string UserId { get; set; }
        public string Text { get; set; }
        public int CommentId { get; set; }
    }
}
