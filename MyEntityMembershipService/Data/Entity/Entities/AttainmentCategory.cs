namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class AttainmentCategory : BaseEntity
    {
        public string Name { get; set; }

        public string ArabicName { get; set; }
        public ICollection<Attainment> Attainmenties { get; set; } = new HashSet<Attainment>();

    }
}
