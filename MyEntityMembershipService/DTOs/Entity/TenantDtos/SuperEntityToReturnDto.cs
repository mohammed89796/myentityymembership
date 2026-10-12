namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class SuperEntityToReturnDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string? Image { get; set; }

        public int? EntityTypeId { get; set; }


    }
}
