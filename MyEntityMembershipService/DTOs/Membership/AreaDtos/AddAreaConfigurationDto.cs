namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class AddAreaConfigurationDto
    {
        public int AreaId { get; set; }
        public int OperationId { get; set; } // Request Type Id
        public int FormId { get; set; }
        public int AreaFee { get; set; }
        public int AreaVAT { get; set; }
        public int AreaOperatingFees { get; set; }
    }
}
