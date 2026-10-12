namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Like : BaseEntity
    {
        public Post Post { get; set; }

        public int PostId { get; set; }
    }
}
