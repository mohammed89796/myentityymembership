namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class FieldCategory: BaseEntity
    {
        public string Name { get; set; }
        public List<RequestTypeFields> RequestTypeFields { get; set; }
    }
}
