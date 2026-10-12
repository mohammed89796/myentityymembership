namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class UserChoice : BaseEntity
    {
        public Choice Choice { get; set; }

        public int ChoiceId { get; set; }
    }
}
