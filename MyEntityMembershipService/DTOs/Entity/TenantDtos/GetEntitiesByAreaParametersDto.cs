namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class GetEntitiesByAreaParametersDto
    {
        public int AreaId { get; set; }
        public int? FederationId { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }
}
