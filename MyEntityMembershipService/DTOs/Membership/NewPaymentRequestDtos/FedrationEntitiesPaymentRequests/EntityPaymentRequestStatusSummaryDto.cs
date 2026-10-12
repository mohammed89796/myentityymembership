namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests
{
    public class EntityPaymentRequestStatusSummaryDto
    {
        public string EntityName { get; set; }
        public string? Image { get; set; }
        public int TotalRequests { get; set; }
        public int UnpaidRequests { get; set; }
        public int PaidRequests { get; set; }
        public int PendingPayment { get; set; }
        public int PendingConfirmation { get; set; }
        public int ConfirmedRequests { get; set; }
    }
}
