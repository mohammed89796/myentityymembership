namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class PackageBranchListDto
    {
        public int  PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
    }
}
