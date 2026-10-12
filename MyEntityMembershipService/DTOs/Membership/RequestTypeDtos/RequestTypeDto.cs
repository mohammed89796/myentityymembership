namespace MyEntityMembershipService.DTOs.Membership.RequestTypeDtos
{
    public class RequestTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RequestTypeCategoryId { get; set; }

    }
}
