namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Attainment : BaseEntity
    {
        public string Title { get; set; }

        public AttainmentCategory AttainmentCategory { get; set; }
        public int AttainmentCategoryId { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public string IssuingOrganization { get; set; }

        public DateOnly IssuingDate { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public string? AttachedDocument { get; set; }

    }
}
