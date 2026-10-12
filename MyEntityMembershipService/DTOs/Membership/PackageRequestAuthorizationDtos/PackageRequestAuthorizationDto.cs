namespace MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos
{
    public class PackageRequestAuthorizationDto
    {
		public int PackageId { get; set; }
		public int RequestAuthourizationId { get; set; }
        public string? RequestAuthourizationName { get; set; }

        public PackageRequestTypeDto? RequestType { get; set; }
    }
}
