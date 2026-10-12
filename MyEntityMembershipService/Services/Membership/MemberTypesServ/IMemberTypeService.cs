using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.MemberTypesServ
{
    public interface IMemberTypeService
    {
        Task<Response<List<MemberTypeDto>>> GetAllTypes(string? Name, int? requestTypeId);
        Task<Response<MemberTypeDto>> AddMemberType(MemberTypeDto memberType);
        Task<Response<MemberTypeDto>> GeTypeById(int id);
        Task<Response<List<MemberTypeDto>>> GetAllTypeExceptMain();

    }
}
