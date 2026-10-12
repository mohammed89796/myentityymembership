namespace MyEntityMembershipService.Authorization.Permissions.Dto
{
    public class AuthorizationRequestDto
    {
        public string UserId { get; set; }
        public string PermissionCode { get; set; }
        public List<string> PoliciesName { get; set; } = [];
    }
}
