namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class UpdateAreaPricingDto
    {
        public int Id { get; set; }
        public int MembershipPrice { get; set; }
        public int TransferPrice { get; set; }
        public int LoanPrice { get; set; }
    }
}
