namespace MyEntityMembershipService.DTOs.Entity.AttainmentDtos
{
    public class AddAttainmentDto
    {
        public string Title { get; set; }
        public int AttainmentCategoryId { get; set; }
        public string IssuingOrganization { get; set; }
        public DateOnly IssuingDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public IFormFile? AttachedDocument { get; set; }
    }
}
