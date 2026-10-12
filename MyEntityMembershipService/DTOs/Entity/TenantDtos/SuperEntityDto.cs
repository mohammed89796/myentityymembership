namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class SuperEntityDto
    {
        public string? SearchTerm { get; set; }

        public int? EntityTypeId { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }


    }
}
