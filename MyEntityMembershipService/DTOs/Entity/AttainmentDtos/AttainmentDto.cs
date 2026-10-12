namespace MyEntityMembershipService.DTOs.Entity.AttainmentDtos
{
    public class AttainmentDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;

        public string? SearchTerm { get; set; }

        public bool? IsActive { get; set; }

    }
}
