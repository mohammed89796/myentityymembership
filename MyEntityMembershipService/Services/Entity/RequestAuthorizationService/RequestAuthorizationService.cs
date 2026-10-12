//using AutoMapper;
//using MyEntity.Data.Entities;
//using MyEntity.DTOs.PackageDtos;
//using MyEntity.Repositories.Common;
//using MyEntity.Repositories.UnitOfWorkRepo;



//namespace MyEntity.Services.RequestAuthorizationServ
//{
//    public class RequestAuthorizationService : IRequestAuthorizationService
//    {
//        private readonly IUnitOfWork unitOfWork;
//        private readonly IMapper mapper;

//        public RequestAuthorizationService(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            this.unitOfWork = unitOfWork;
//            this.mapper = mapper;
//        }

//        public async Task<Response<RequestAuthorizationDto>> AddRequestAuthorization(RequestAuthorizationDto requestAuthorizationDto)
//        {
//            try
//            {
//                var requestAuthorization = mapper.Map<RequestAuthourization>(requestAuthorizationDto);
//                await unitOfWork.Repository<RequestAuthourization>().Add(requestAuthorization);
//                await unitOfWork.Save();
//                requestAuthorizationDto = mapper.Map<RequestAuthorizationDto>(requestAuthorization);
//                return new ResponseHandler().Success(requestAuthorizationDto);
//            }
//            catch (Exception ex)
//            {
//                return new ResponseHandler().BadRequest<RequestAuthorizationDto>("Something went wrong");
//            }
//        }

//        public async Task<Response<List<RequestAuthorizationDto>>> GetAllRequestAuthorizations()
//        {
//            try
//            {
//                var requestAuthorizations = await unitOfWork.Repository<RequestAuthourization>().GetAllAsync();
//                var requestAuthorizationDtos = mapper.Map<List<RequestAuthorizationDto>>(requestAuthorizations);
//                return new ResponseHandler().Success(requestAuthorizationDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ResponseHandler().BadRequest<List<RequestAuthorizationDto>>("Something went wrong");
//            }
//        }
//    }
//}
