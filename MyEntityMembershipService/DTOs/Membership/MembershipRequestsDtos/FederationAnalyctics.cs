namespace MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos
{
    public class GetFedEntityAnalytics
    {
        public string NumberOfBatches { get; set; }
        public string NumberOfRequests { get; set; }
        public string ApprovedAthletes { get; set; }
        public string RejectedAthletes { get; set; }
        public string PendingAthletes { get; set; }
        
    }
    public class GetBatchAnalytics
    {
        public string BatchNumber { get; set; }
        public string EntityType { get; set; }
        public string BranchName { get; set; }
        public DateOnly? DateOfSubmission { get; set; }
        public string TotalRequests { get; set; }
        public string ApprovedAthletes { get; set; }
        public string RejectedAthletes { get; set; }
        public string PendingAthletes { get; set; }
        
    }
}
