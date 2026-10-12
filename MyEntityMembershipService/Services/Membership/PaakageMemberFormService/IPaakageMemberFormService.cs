using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.PaakageMemberFormService
{
    public interface IPaakageMemberFormService
    {
        Task<Response<List<PaakageMemberFormDto>>> AddPaakageMemberForms(List<PaakageMemberFormDto> dtos);
        Task<Response<PaakageMemberFormDto>> UpdatePaakageMemberForm(PaakageMemberFormDto dto);
        Task<Response<bool>> DeletePaakageMemberForm(int id);
        Task<Response<List<PaakageMemberFormDto>>> GetAllPaakageMemberForms();
        Task<Response<PaakageMemberFormDto>> GetPaakageMemberFormById(int id);
        Task<Response<List<PaakageMemberFormDto>>> SyncPaakageMemberForms(int packageId, List<PaakageMemberFormDto> newForms);


    }

}
