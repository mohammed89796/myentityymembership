
using MyEntityMembershipService.DTOs.Entity.SystemDtos.StateDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.System.StateRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Entity.System.StateService
{
    public class StateService : IStateService
    {
        private readonly IStateRepo _stateRepo;
        private readonly IUnitOfWork _unitOfWork;


        public StateService(IUnitOfWork unitOfWork, IStateRepo stateRepo)
        {
            _unitOfWork = unitOfWork;
            _stateRepo = stateRepo;
        }


        public async Task<Response<bool>> CreateState(int countryID, string stateName)
        {
            try
            {
                State state = new State
                {
                    Name = stateName,
                    CountryID = countryID
                };

                await _unitOfWork.Repository<State>().Add(state);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");

            }
        }


        public async Task<Response<List<StateViewModel>>> GetAllStates(int countryID)
        {
            try
            {
                var result = await _stateRepo.GetStates(countryID);

                if (result == null || !result.Any())
                {
                    return new ResponseHandler().Success(new List<StateViewModel>(), "No states found.");
                }

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<StateViewModel>>("Something went wrong");
            }
        }
    }

}
