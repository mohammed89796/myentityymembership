using MyEntityMembershipService.DTOs.Entity.AtheleteDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.AtheleteService
{
    public interface IAtheleteService
    {
        Task<Response<int>> CreateAtheleteAsync(AddAtheleteDto atheleteDto);

        Task<Response<List<AtheleteToReturnDto>>> GetDraftAtheleteListAsync(int tenantId);
    }
}
