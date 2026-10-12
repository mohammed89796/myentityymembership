namespace MyEntityMembershipService.DTOs.Entity.FederationDtos
{
    public class AddFederationEntityDto
    {
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        public int FedrationBranchId { get; set; }
        public int EntityTypeId { get; set; }
        public int SuperEntityId { get; set; }
    }
}
