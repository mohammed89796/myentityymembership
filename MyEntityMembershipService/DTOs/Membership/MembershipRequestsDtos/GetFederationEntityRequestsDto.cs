using MyEntityMembershipService.Data.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class PaginatedFedEntityRequestsList
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<GetFedEntityRequestsDto> Data { get; set; }
    }
    public class PaginatedFedRequestsList
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<GetFedRequestsDto> Data { get; set; }
    }
    public class GetFedRequestsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Entity { get; set; }
        public string Branch { get; set; }
        public string FileNumber { get; set; }
        public string NationalID { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string BeltLevel { get; set; }
        public string DateOfAcquisition { get; set; }
        public int RequestStatus { get; set; }
        public int PaymentStatus { get; set; }

        public BranchAdminStatus? BranchAdminStatus { get; set; }
        public BranchAdminStatus? PaymentBranchAdminStatus { get; set; }
    }
    public class GetFedEntityRequestsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileNumber { get; set; }
        public DateOnly DateOfSubmission { get; set; }
        public string NationalID { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string BeltLevel { get; set; }
        public int PaymentRequestID { get; set; }
        public string DateOfAcquisition { get; set; }
        public int RequestStatus { get; set; }
        public int PaymetStatus { get; set; }
        public BranchAdminStatus BranchAdminStatus { get; set; }
        public BranchAdminStatus PaymentBranchAdminStatus { get; set; }
    }
    public class FedEntityRequestFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? BeltLevel { get; set; }
        public int? RequestStatus { get; set; }
        public int? PaymentStatus { get; set; }
        public int? BranchRequestStatus { get; set; }
        public int? BranchPaymentStatus { get; set; }
        public string? SearchData { get; set; }
    }
    public class FedRequestFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateOnly? Date { get; set; }
        public string? BeltLevel { get; set; }
        public int? RequestStatus { get; set; }
        public int? PaymentStatus { get; set; }
        public string? SearchData { get; set; }
    }
    public class FedEntityRequestByBatchFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? BeltLevel { get; set; }
        public DateOnly? Date { get; set; }
        public int? RequestStatus { get; set; }
        public int? PaymentStatus { get; set; }
        public string? SearchData { get; set; }

    }


}
