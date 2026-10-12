using MyEntityMembershipService.DTOs.Entity;

namespace MyEntityMembershipService.DTOs.Entity.MobileDtos
{
    public class MobilePostToReturnDto
    {
        public int Id { get; set; }

        public string PostTitle { get; set; }

        public string Description { get; set; }

        public string? PostPictureUrl { get; set; }

        public DateOnly PublishDate { get; set; }

        public int TenantId { get; set; }

        public string TenantName { get; set; }

        public UserDto PostedBy { get; set; }

        public bool IsBookMarked { get; set; }

        public int TotalNumberOfComments { get; set; }

    }
}
