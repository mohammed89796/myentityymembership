using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class MembershipRequestDetailsDto
    {
        public int RequestId { get; set; }
        public string Name { get; set; }
        public string ProfileImageURL { get; set; }
        public string FileNumber { get; set; }
        public int PaymentRequestID { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public BranchAdminStatus? BranchAdminStatus { get; set; }
        public BranchAdminStatus? PaymentBranchAdminStatus { get; set; }
        public RejectionReason? RejectionReason { get; set; }
        public string? TransferredFrom { get; set; }
        public string? TransferredTo { get; set; }



        public List<CategoryGroupDto> CategorizedResponses { get; set; } = new();
    }
    public class MembershipRequestDetailsByBatchDto
    {
        public int RequestId { get; set; }
        public string ProfileImageURL { get; set; }
        public string Name { get; set; }
        public string FileNumber { get; set; }

        public List<CategoryGroupDto> CategorizedResponses { get; set; } = new();
    }

    public class CategoryGroupDto
    {
        public string Category { get; set; }
        public List<DetailedFieldResponseDto> Fields { get; set; }
    }

    public class DetailedFieldResponseDto
    {
        public string Title { get; set; }
        public string Value { get; set; }

    }



}
