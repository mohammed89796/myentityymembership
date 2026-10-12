using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.AttainmentDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.AttainmentService
{
    public interface IAttainmentService
    {
        Task<Response<bool>> CreateAttainmentAsync(AddAttainmentDto dto, string tenantId);

        Task<Response<bool>> DeleteAttainment(int attainmentId);

        Task<Response<bool>> UpdateAttainmentAsync(UpdateAttainmentDto dto);

        Task<Response<AttainmentToReturnDto>> GetAttainmentByIdAsync(int attainmentId);
 
        Task<Response<PaginationToReturnDto<AttainmentToReturnDto>>> GetAllAttainmentsAsync(AttainmentDto dto, string tenantId);
 
        Task<Response<List<object>>> GetAllAttainmentCategoriesAsync();
        



    }
}
