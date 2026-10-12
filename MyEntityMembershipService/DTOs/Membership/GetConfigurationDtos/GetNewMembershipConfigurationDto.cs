namespace MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos
{
    public class GetNewMembershipConfigurationDto : IBaseConfigurationDto
    {
        public int PackageId { get; set; }
        public int FormID { get; set; }
    }
}
