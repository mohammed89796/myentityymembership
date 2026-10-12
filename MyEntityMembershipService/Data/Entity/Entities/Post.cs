using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Post : BaseEntity
    {
        public string PostTitle { get; set; }

        public string? Description { get; set; }

        public List<string>? PostPictureUrl { get; set; } = new List<string>();

        public DateOnly? PublishDate { get; set; }

        public PostType PostType { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public Poll? Poll { get; set; }
        public int? PollId { get; set; }

        public ICollection<PostTag> PostTags { get; set; } = new HashSet<PostTag>();
        public ICollection<BookMark> BookMarks { get; set; } = new HashSet<BookMark>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<Share> Shares { get; set; } = new HashSet<Share>();
        public ICollection<Like> Likes { get; set; } = new HashSet<Like>();

        public Priority? Priority { get; set; }

        public bool IsDraft { get; set; }
        public bool IsPinned { get; set; }

        public bool HasLikes { get; set; }
        public bool HasComments { get; set; }
        public bool HasShares { get; set; }


    }
}
