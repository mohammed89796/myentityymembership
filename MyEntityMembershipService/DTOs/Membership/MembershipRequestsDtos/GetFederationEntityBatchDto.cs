namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class PaginatedFederationEntityBatchList
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<GetFederationEntityBatchDto> Data { get; set; }
    }

    public class GetFederationEntityBatchDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; }
        public DateOnly DateOfSubmission { get; set; }
        public int TotalAthletes { get; set; }
        public int ApprovedAthletes { get; set; }
        public int RejectedAthletes { get; set; }
        public int PendingAthletes { get; set; }
    }

    public class FedEntityBatchFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateOnly? Date { get; set; }
        public string? SearchData { get; set; }

    }


}
