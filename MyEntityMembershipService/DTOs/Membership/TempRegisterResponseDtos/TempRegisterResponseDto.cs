namespace MyEntityMembershipService.DTOs.Membership.TempRegisterResponseDtos
{
    public class TempRegisterResponseDto
    {
        public string UserId { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
