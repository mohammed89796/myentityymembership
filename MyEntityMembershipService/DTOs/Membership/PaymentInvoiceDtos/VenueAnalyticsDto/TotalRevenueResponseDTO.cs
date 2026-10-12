namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos.VenueAnalyticsDto
{
    public class TotalRevenueResponseDTO
    {
        public double MaxRevenue { get; set; }
        public double MinRevenue { get; set; }
        public int Interval { get; set; }
        public List<TotalFacilityRevenueDTO> FacilityRevenue { get; set; }

    }

}
