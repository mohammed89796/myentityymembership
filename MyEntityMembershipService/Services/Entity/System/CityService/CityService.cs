
using MyEntityMembershipService.DTOs.Entity.SystemDtos.CityRepo;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.System.CityRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Entity.System.CityService
{
    public class CityService : ICityService
    {
        private readonly ICityRepo _cityRepo;
        private readonly IUnitOfWork _unitOfWork;


        public CityService(IUnitOfWork unitOfWork, ICityRepo cityRepo)
        {
            _unitOfWork = unitOfWork;
            _cityRepo = cityRepo;
        }


        public async Task<Response<bool>> CreateCity(int stateID, string cityName)
        {
            try
            {
                City city = new City
                {
                    Name = cityName,
                    StateID = stateID
                };

                await _unitOfWork.Repository<City>().Add(city);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");

            }
        }

        public async Task<Response<List<CityViewModel>>> GetAllCity(int stateID)
        {
            try
            {
                var result = await _cityRepo.GetCities(stateID);

                if (result == null || !result.Any())
                {
                    return new ResponseHandler().Success(new List<CityViewModel>(), "No cities found.");
                }

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<CityViewModel>>("Something went wrong");
            }
        }

    }

}
