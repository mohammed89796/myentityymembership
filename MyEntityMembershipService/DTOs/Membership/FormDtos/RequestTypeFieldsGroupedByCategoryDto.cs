namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class RequestTypeFieldsGroupedByCategoryDto
    {
        public int FieldCategoryId { get; set; }
        public string FieldCategoryName { get; set; }
        public List<FieldDto> Fields { get; set; }
    }
}
