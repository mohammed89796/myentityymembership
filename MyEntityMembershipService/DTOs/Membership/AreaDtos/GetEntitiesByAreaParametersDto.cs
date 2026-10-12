namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class GetEntitiesByAreaParametersDto
    {
        public int AreaId { get; set; }
        public int? FederationId { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int? EntityTypeId { get; set; }
        public string? EntityName { get; set; }  
    }
}
