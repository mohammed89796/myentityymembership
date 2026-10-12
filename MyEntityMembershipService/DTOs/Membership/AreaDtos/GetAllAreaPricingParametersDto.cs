namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class GetAllAreaPricingParametersDto
    {
        public int AreaId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchBy { get; set; }

    }
}
