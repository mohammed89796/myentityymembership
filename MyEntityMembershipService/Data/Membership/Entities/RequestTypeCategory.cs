namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class RequestTypeCategory : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<RequestType> RequestTypes { get; set; }
    }
}
