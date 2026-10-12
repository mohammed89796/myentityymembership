using MyEntityMembershipService.DTOs.Entity.SystemDtos.StateDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.System.StateService
{
    public interface IStateService
    {
        public Task<Response<bool>> CreateState(int countryID, string cityName);
        public Task<Response<List<StateViewModel>>> GetAllStates(int countryID);

    }
}