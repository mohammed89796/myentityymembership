namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class AreaConfigurationDto
    {
        public int Id { get; set; }
        public int OperationId { get; set; } // Request Type Id
        public int FormId { get; set; }
        public string FormName { get; set; }
        public double FormFee { get; set; }
        public int AreaFee { get; set; }
        public int AreaVAT { get; set; }
        public int AreaOperatingFees { get; set; }
    }
}
