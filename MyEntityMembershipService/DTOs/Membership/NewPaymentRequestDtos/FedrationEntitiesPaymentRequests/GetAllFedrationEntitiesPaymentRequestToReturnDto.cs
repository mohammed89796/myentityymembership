namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests
{
    public class GetAllFedrationEntitiesPaymentRequestToReturnDto
    {
        public string EntityName { get; set; }
        public string? BranchName { get; set; }
        public int Id { get; set; }
        public int NumberOfRequests { get; set; }
        public int RequestsWithoutPayment { get; set; }
        public int RequestsWithPayment { get; set; }


    }
}
