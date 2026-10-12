namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Sports : BaseEntity
    {
        public string SportName { get; set; }
        public virtual ICollection<Team> Teams { get; set; }=new HashSet<Team>();

    }
}
