namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class ReturnRequestTypesGrouped
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ReturnRequestTypeDto> RequestTypes { get; set; }
    }
}
