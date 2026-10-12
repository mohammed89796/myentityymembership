namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Share : BaseEntity
    {
        public Post Post { get; set; }

        public int PostId { get; set; }
    }
}
