

namespace MyEntityMembershipService.DTOs.Entity.FederationDtos
{
    public class FederationEntityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }

        public int? EntityTypeId { get; set; }
        public int? FedrationEntityTypeId { get; set; }

        public int? FedrationBranchId { get; set; }

        public string? FedrationBranchName { get; set; }

        public string? EntityTypeName { get; set; }

        public int SuperEntityId { get; set; }
        public string? TypeName { get; set; }

        public int TenantId { get; set; }



    }
}
