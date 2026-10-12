namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests
{
    public class FederationPaymentSummaryDto
    {
     
        public int NumberOfEntities { get; set; }
        public int NumberOfRequests { get; set; }
        public int RequestsWithoutPayment { get; set; }
        public int RequestsWithPayment { get; set; }
    }
}
