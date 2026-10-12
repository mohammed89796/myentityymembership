using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PreRegisteredUser : BaseEntity
    {

        public string FileNumber { get; set; }
        public string? NationalId { get; set; }
        public string? FullName { get; set; }
        public string? FullNameArabic { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PassportNumber { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public DateOnly? RegisterationDate { get; set; }
        public string? RegisterationSerialNumber { get; set; }
        public string BeltName { get; set; }
        public DateOnly? KarateBeltDate { get; set; }
        public string? OtherContactInfo { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

    }
}
