

namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class AreaPricingDto
    {
        public int Id { get; set; }
        public string BeltName { get; set; }
        public int BeltId { get; set; }
        public int MembershipPrice { get; set; }
        public int TransferPrice { get; set; }
        public int LoanPrice { get; set; }

    }
}
