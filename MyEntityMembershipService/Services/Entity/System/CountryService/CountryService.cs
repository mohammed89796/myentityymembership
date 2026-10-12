
using MyEntityMembershipService.DTOs.Entity.SystemDtos.CountryDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.System.CountryRep;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Entity.System.CountryService
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepo _countryRepo;
        private readonly IUnitOfWork _unitOfWork;


        public CountryService(IUnitOfWork unitOfWork, ICountryRepo countryRepo)
        {
            _unitOfWork = unitOfWork;
            _countryRepo = countryRepo;
        }



        public async Task<Response<bool>> CreateCountry(string countryName)
        {
            try
            {
                Country country = new Country
                {
                    Name = countryName,
                };

                await _unitOfWork.Repository<Country>().Add(country);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");

            }
        }

        public async Task<Response<List<CountryViewModel>>> GetAllCountries()
        {
            try
            {
                var result = await _countryRepo.GetCountries();

                if (result == null || !result.Any())
                {
                    return new ResponseHandler().Success(new List<CountryViewModel>(), "No countries found.");
                }

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<CountryViewModel>>("Something went wrong");
            }
        }

    }

}
