namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class GetBranchEntityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? EntityTypeId { get; set; }
        public string EntityTypeName { get; set; }
    }
}
