namespace MyEntityMembershipService.DTOs.Membership.TempRegisterResponseDtos
{
    public class UpdateTempUserRequestDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int TenantId { get; set; }
        public string? NationalityId { get; set; }
    }
}
