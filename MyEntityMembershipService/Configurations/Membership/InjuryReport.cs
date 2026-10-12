
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class InjuryReport:BaseEntity
    {
        public string SwimmerNationalId { get; set; }
        public string SwimmerPhoneNumber { get; set; }
        public string? InjuryName { get; set; }
        public InjuryStatus? InjuryStatus { get; set; }
        public string? Description { get; set; }

        public DateTime InjuryDate { get; set; }
        public DateTime? EstimatedRecoveryDate { get; set; }

        public int? MembershipId { get; set; }
        public Membership? Membership { get; set; }
    }

}
