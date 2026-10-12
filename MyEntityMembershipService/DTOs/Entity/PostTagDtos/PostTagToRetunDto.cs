namespace MyEntityMembershipService.DTOs.Entity.PostTagDtos
{
    public class PostTagToRetunDto
    {
        public int Id { get; set; }

        public string Tag { get; set; }

        public int PostId { get; set; }

        public string PostTitle { get; set; }
    }
}
