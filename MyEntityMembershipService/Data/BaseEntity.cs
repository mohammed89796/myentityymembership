namespace MyEntityMembershipService.Data
{
    public class BaseEntity : SharedEntity
    {
        public int Id { get; set; }
        public int GlobalTenantId { get; set; }


    }
}
