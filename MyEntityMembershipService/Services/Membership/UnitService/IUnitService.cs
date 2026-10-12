
using MyEntityMembershipService.DTOs.Membership.UnitDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.UnitService
{
    public interface IUnitService
    {
        public Task<Response<List<UnitDto>>> GetAllAsync();
        public Task<Response<UnitDto>> GetByIdAsync(int id);
        public Task<Response<UnitDto>> GetByNameAsync(string name);
        public Task<Response<List<UnitDto>>> GetByUnitTypeIdAsync(int unitTypeId);

        public Task<Response<UnitDto>> AddAsync(AddUnitDto dto);
        public Task<Response<UnitDto>> UpdateAsync(UnitDto dto);
        public Task<Response<bool>> Delete(int id);
    }
}
