namespace MyEntityMembershipService.DTOs.Membership.DashboardDtos
{
    public class DashboardSummaryDto
    {
        public IEnumerable<PaymentSummaryDto> TotalProgramPayment { get; set; }
        public IEnumerable<PaymentSummaryDto>? TotalMembershipPayment { get; set; }
        public IEnumerable<PaymentSummaryDto>? TotalVenueBookingPayment { get; set; }
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 250;
        public int Interval { get; set; } = 50;

    }
}
