namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MembershipRequestAccess:BaseEntity
    {
        public int MembershipRequestId { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int? FacilityId { get; set; }
        public NewFacility? Facility { get; set; }
        public int? VenueId { get; set; }

        public NewVenue? Venue { get; set; }

        public MembershipRequest MembershipRequest { get; set; }
    }
}
