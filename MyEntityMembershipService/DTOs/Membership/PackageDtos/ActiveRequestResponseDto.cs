using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{
    public class ActiveRequestResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }

        public string? MemberType { get; set; }
        public string? RequestType { get; set; }
        public DateTime SubmittedAt { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
        public RequestStatus Status { get; set; }
        public RequestPaymentStatus PaymentStatus { get; set; }


    }

    public class ActiveRequestFilter
    {
        public int? TenantId { get; set; }
        public int? RequestTypeFilterId { get; set; }
        public string? SearchTerm { get; set; }
        public PaymentProceduresType? PaymentProceduresTypeFilter { get; set; }
        public RequestStatus? StatusFilter { get; set; }
        public RequestPaymentStatus? PaymentStatusFilter{ get; set; }
    }
}
