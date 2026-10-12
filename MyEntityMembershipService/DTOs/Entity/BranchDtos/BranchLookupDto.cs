namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class BranchLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BranchLookupSearchDto
    {
        public int? TenantId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
