namespace MyEntityMembershipService.DTOs.Entity.EntityAdminDtos
{
    public class GetEntityAdminsParametersDto
    {
        public int? TenantId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? Status { get; set; }
        public string? SearchBy { get; set; }
    }
}
