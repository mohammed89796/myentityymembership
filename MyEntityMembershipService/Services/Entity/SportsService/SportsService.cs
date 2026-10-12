using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.SportsRepo;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;


namespace MyEntityMembershipService.Services.Entity.SportsService
{
    public class SportsService : ISportsService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ISportsRepository _sportsRepository;

        public SportsService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ISportsRepository sportsRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _sportsRepository = sportsRepository;
        }

        public async Task<Response<bool>> CreateSport(string sportName)
        {
            try
            {
                var sport = new Sports
                {
                    SportName = sportName
                };

                await _unitOfWork.Repository<Sports>().Add(sport);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<Sports>>> GetSportsDropDown()
        {
            try
            {

                 var coreServiceList = await _sportsRepository.GetAllSportsAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<Sports>>("Sport list not found");
                }

                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<Sports>>("Something went wrong");
            }
        }


    }
}
