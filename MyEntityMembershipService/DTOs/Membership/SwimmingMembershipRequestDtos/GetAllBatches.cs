using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.SwimmingMembershipRequestDtos
{
    public class GetAllBatches
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; }
        public BatchStatus BatchStatus { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateOfSubmission { get; set; }
        public SubmationPlatform SubmationPlatform { get; set; }
        public int TotalAthletes { get; set; }
        public int PendingAthletes { get; set; }
        public int ApprovedAthletes { get; set; }
        public int RejectedAthletes { get; set; }

    }
}
