using MyEntityMembershipService.DTOs.Membership.FieldsDto;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.FieldServ
{
    public interface IFieldService
    {
        public Task<Response<List<FieldDto>>> GetAllFields();
        Task<Response<FieldDto>> GetFieldById(int id);
        Task<Response<FieldDto>> CreateField(CreateFieldDto dto);
        Task<Response<FieldDto>> UpdateField(UpdateFieldDto dto);
        Task<Response<string>> DeleteField(int id);
    }
}
