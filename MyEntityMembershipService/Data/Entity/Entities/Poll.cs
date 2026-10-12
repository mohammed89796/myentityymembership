namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Poll : BaseEntity
    {
        public Post Post { get; set; }

        public int PostId { get; set; }

        public bool HasMultiSelect { get; set; }

        public string Question { get; set; }

        public ICollection<Choice> Choices { get; set; } = new HashSet<Choice>();
    }
}
