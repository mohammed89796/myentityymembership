namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class GetBranchEntityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? EntityTypeId { get; set; }
        public string EntityTypeName { get; set; }
        public int? NumberOfMembers { get; set; }
    }
}
