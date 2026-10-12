namespace MyEntityMembershipService.DTOs.Entity.EntityAdminDtos
{
    public class GetTenantCoachAdminsParametersDto
    {
        public string? Search { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
