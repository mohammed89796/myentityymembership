namespace MyEntityMembershipService.DTOs.Entity.AttainmentDtos
{
    public class AttainmentToReturnDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int AttainmentCategoryId { get; set; }

        public string AttainmentCategoryName { get; set; }

        public string AttainmentCategoryArabicName { get; set; }

        public string IssuingOrganization { get; set; }

        public DateOnly IssuingDate { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public string? AttachedDocumentUrl { get; set; }

        public int TenantId { get; set; }

        public bool IsActive { get; set; }


    }
}
