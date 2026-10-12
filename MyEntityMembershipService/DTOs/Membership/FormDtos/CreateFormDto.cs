namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class CreateFormDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public double Fees { get; set; }
        public string WorkflowSelection { get; set; }
        public List<FormFieldsDto> FieldsDto { get; set; }
        public int RequestTypeId { get; set; }
        public string? CreatedBy { get; set; }
        public List<int>? MemberTypesId { get; set; } = new List<int>();

    }
}
