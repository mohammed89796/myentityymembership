namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class PaginatedFederationBatchList
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<GetFederationBatchDto> Data { get; set; }
    }
    public class GetFederationBatchDto
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string Branch { get; set; }
        public string BatchNumber { get; set; }
        public DateOnly DateOfSubmission { get; set; }
        public int TotalAthletes { get; set; }
        public int ApprovedAthletes { get; set; }
        public int RejectedAthletes { get; set; }
        public int PendingAthletes { get; set; }
    }
    public class FedBatchFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TenentID { get; set; }
        public int AreaID { get; set; }
        public DateOnly? Date { get; set; }
        public string? SearchData { get; set; }

    }


}
