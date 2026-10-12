namespace MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos
{
    public class GetFreezeConfigurationDto : IBaseConfigurationDto
    {
        public int FreezeLimit { get; set; }
        public double PricePerAdditionalDay { get; set; }

        public string UnitName { get; set; }
        public int MembershipFreezeDaysRemaining { get;  } = 0;
    }
}
