
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;

namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class FormDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? WorkflowSelection { get; set; }
        public RequestTypeDto RequestType { get; set; }
        public List<FieldDto> Fields { get; set; }
    }
}
