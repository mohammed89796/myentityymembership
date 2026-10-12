namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class UnitType : BaseEntity
    {
        public string Name { get; set; }
        public List<Unit> Units { get; set; }
        public List<Field> Fields { get; set; }
    }
}
