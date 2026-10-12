namespace MyEntityMembershipService.DTOs.Membership.FederationEntitiesDtos
{
    public class GetFederationEntitiesParameterDto
    {
        public string? userId { get; set; }
        public int tenentId { get; set; }
        public int? pageNumber { get; set; } = 1;
        public int? pageSize { get; set; } = 10;
        public int? BranchId { get; set; }
        public int? EntityTypeId { get; set; }
        public string? SearchBy { get; set; }
    }
}
