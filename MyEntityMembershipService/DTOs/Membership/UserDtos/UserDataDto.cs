using MyEntityMembershipService.DTOs.Membership.TenantDtos;

namespace MyEntityMembershipService.DTOs.Membership.UserDtos
{
    public class UserDataDto
    {
        public TenantDataDto TenantData { get; set; }= new TenantDataDto();

        public List<UserInfoDto> Users { get; set; } = [];

    }
}
