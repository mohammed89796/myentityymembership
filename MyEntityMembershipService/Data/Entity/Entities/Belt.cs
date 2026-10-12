namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Belt : BaseEntity
    {
        public string Name { get; set; }

        public string ArabicName { get; set; }
        public ICollection<EntityMember> EntityMembers { get; set; } = new HashSet<EntityMember>();
    }


}
