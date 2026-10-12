namespace MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos
{
    public class ConfirmedMembershipRequestDto
    {
        public string FullName { get; set; }
        public string MembershipId { get; set; }
        public string PhoneNumber { get; set; }
        public string BeltName { get; set; }
        public string MemberSince { get; set; } // date only
        public string MembershipStartDate { get; set; } // date only
        public string MembershipExpiryDate { get; set; } // date only
        public string MembershipStatus { get; set; }
    }
}
