namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class FormFieldsDto
    {
        public int FieldId { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsEditable { get; set; }
        public bool? IsAdminFilled { get; set; }
        public int Index { get; set; }

    }
}
