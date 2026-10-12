namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Comment : BaseEntity
    {
        public string Text { get; set; }

        public int? PostId { get; set; }
        public Post Post { get; set; }


        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }


        public ICollection<Comment> Replies { get; set; } = new HashSet<Comment>();
    }
}
