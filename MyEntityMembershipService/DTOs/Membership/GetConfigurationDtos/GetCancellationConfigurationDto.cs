namespace MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos
{
    public class GetCancellationConfigurationDto : IBaseConfigurationDto
    {
        public int? CancellationFees { get; set; } = 0;
    }
}
