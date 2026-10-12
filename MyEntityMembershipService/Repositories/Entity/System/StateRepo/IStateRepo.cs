
using MyEntityMembershipService.DTOs.Entity.SystemDtos.StateDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.StateRepo
{
    public interface IStateRepo : IGenericRepository<State>
    {
        public Task<List<StateViewModel>> GetStates(int countryID);

    }
}
