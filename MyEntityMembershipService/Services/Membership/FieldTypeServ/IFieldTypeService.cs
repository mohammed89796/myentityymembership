using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.FieldTypeServ
{
    public interface IFieldTypeService
    {
        Task<Response<List<FieldTypeDto>>> GetAllTypes();
        Task<Response<FieldTypeDto>> AddFieldType(FieldTypeDto fieldType);
    }
}
