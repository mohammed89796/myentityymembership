namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos.VenueAnalyticsDto
{
    public class TotalFacilityRevenueDTO
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public List<TotalRevenueDTO> Revenue { get; set; }
    }

}
