namespace MyEntityMembershipService.DTOs.Membership.UserDtos
{
    public class UserInfoDto
    {
        public string BeltName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int? SportId { get; set; }
        public string? SportName { get; set; }
        public string? SportImageUrl { get; set; }
        public string? CountryPlayedIn { get; set; }
    }
}
