using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.SwimmingMembershipRequestDtos
{
    public class GetAllChildRequestParametersDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; } // Name, NationalId, Email, Phone
        public Gender? Gender { get; set; }
        public string? Level { get; set; }
        public bool? OnlyDraft { get; set; }  

    }
}
