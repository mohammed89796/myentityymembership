using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

using MyEntityMembershipService.Repositories.Entity.CountryCodeRepo;


namespace MyEntityMembershipService.Services.Entity.CountryCode
{
    public class CountryCodesService : ICountryCodesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ICountryCodesRepository _countryCodeRepository;


        public CountryCodesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ICountryCodesRepository countryCodeRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _countryCodeRepository = countryCodeRepository;

        }

        public async Task<Response<bool>> CreateCountryCode(string code, IFormFile Flag)
        {
            try
            {

                var url = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", Flag);

                var countryCode = new CountryCodes
                {
                    FlagURL = url,
                    Code = code
                };

                await _unitOfWork.Repository<CountryCodes>().Add(countryCode);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<CountryCodes>>> GetCountryCodesDropDown()
        {
            try
            {

                var countryCodesList = await _countryCodeRepository.GetAllCountryCodesAsync();

                if (countryCodesList is null)
                {
                    return new ResponseHandler().NotFound<List<CountryCodes>>("Country Codes list not found");
                }

                var tasks = countryCodesList.Select(async dto =>
                {
                    dto.FlagURL = await DocumentSetting.GetPreSignedUrlAsync(dto.FlagURL);
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(countryCodesList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<CountryCodes>>("Something went wrong");
            }
        }


    }
}
