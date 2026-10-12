namespace MyEntityMembershipService.DTOs.Membership.EntityDtos
{
    public class AddEntityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile? Image { get; set; }
        public int FedrationBranchId { get; set; }
        public int EntityTypeId { get; set; }
        public int SuperEntityId { get; set; }

    }
}
