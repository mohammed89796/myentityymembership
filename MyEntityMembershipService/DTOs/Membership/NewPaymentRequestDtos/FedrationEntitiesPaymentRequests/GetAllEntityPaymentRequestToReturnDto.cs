using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests
{
    public class GetAllEntityPaymentRequestToReturnDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string FileNumber { get; set; }
        public string BatchNumber { get; set; }
        public string BranchName { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string? RequestType{ get; set; }
        public BranchAdminStatus? RequestBranchAdminStatus { get; set; }
        public BranchAdminStatus? PaymentBranchAdminStatus { get; set; }
        public DateTime DateOfSubmission { get; set; }



    }
}
