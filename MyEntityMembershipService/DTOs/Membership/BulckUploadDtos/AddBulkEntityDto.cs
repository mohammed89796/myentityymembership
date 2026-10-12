namespace MyEntityMembershipService.DTOs.Entity.FederationDtos
{
    public class AddBulkEntityDto
    {
        public string Name { get; set; }
        public int FedrationBranchId { get; set; }
        public int FedrationEntityTypeId { get; set; }
        public int EntityTypeId { get; set; } 
        public int SuperEntityId { get; set; }

    }
}
