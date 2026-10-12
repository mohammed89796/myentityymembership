using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.SwimmingMembershipRequestDtos
{
    public class GetAllBatchesParamDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public BatchStatus? Status { get; set; } 
        public string? SearchTerm { get; set; } // Name, NationalId, Email, Phone
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
