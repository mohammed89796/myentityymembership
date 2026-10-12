namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<State> State { get; set; }
       
    }
}
