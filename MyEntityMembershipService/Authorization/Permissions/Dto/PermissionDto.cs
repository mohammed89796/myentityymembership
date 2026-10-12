namespace MyEntityMembershipService.Authorization.Permissions.Dto
{
    public record PermissionDto
    {
        public string Service { get; set; }
        public string Entity { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string Method { get; set; } = default!;


    }
}
