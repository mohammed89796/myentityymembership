namespace MyEntityMembershipService.DTOs.Entity.MobileDtos
{
    public class SavedPostsOfUserDto : GetAllPostsDto
    {
        public string UserId { get; set; }

        public DateOnly Date { get; set; }
    }
}
