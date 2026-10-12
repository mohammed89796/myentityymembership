namespace MyEntityMembershipService.DTOs.Membership.TempRegisterResponseDtos
{
    public class TempRegisterRequestDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int TenantId { get; set; }
        public string NationalityId { get; set; }
    }
}