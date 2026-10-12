namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class BookMark : BaseEntity
    {
        public int PostId { get; set; }

        public Post Post { get; set; }

        public string UserId { get; set; }
    }


}
