namespace MyEntityMembershipService.DTOs.Entity.MobileDtos
{
    public class PostWithCommentsToReturnDto : MobilePostToReturnDto
    {
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}
