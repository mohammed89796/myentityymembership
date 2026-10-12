namespace MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos
{
    public class GetRenewalConfigurationDto : IBaseConfigurationDto
    {
        public int? RenewalLimitNumber { get; set; }
        public int RenewalsUsed { get; set; } = 3; // Dummy Data! because We don't have Counter for number of renewals in the database
    }
}
