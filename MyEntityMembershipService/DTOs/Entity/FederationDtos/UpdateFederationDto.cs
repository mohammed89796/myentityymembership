namespace MyEntityMembershipService.DTOs.Entity.FederationDtos
{
    public class UpdateFederationDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
        public int? FedrationBranchId { get; set; }
        public int? FedrationEntityTypeId { get; set; }
        public int? SuperEntityId { get; set; }

    }
}
