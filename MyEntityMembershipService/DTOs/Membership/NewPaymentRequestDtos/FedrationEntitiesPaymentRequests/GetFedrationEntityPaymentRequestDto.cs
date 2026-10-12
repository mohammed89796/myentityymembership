namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests
{
    public class GetFedrationEntityPaymentRequestDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        // Search & Filter Data
        public string? SearchTerm { get; set; }
        public int? BranchId { get; set; }


    }
}
