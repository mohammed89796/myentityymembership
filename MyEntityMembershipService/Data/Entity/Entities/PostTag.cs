namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class PostTag : BaseEntity
    {
        public string Tag { get; set; }

        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}
