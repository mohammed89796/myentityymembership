namespace MyEntityMembershipService.Authorization.Permissions.Dto
{
    public record ServicePermissionsDto
    {
        public string ServiceName { get; set; } = default!;
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}
