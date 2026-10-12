namespace MyEntityMembershipService.DTOs.Entity.EntityAdminDtos
{
    public class GetFederationEntitiesAdminsParametersDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int FederationId { get; set; }
        public int? BranchId { get; set; }
        public int? Status { get; set; }
        public string? SearchBy { get; set; }
    }
}
