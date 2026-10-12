namespace MyEntityMembershipService.DTOs.Membership.TenantDtos
{
    public class TenantDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        public int? FedrationEntityTypeId { get; set; }
        public int? EntityTypeId { get; set; }
        public int? SuperEntityId { get; set; }
        public TenantDetailsDto? SuperEntity { get; set; }

        public int? FedrationBranchId { get; set; }

    }
}
