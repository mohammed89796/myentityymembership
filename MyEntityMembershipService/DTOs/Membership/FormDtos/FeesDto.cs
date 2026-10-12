namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class FeesDto
    {
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public int MemberTypeId { get; set; }
        public string MemberTypeName { get; set; }
        public double? FormFees { get; set; }
        public double? OperationalFees { get; set; }
    }
}
