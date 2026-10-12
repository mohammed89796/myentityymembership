namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Choice : BaseEntity
    {
        public Poll Poll { get; set; }

        public int PollId { get; set; }

        public string Text { get; set; }

        public int Vote { get; set; }

        public ICollection<UserChoice> UserChoices { get; set; } = new HashSet<UserChoice>();
    }
}
