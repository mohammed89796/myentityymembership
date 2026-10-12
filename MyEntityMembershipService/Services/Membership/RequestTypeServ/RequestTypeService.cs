using AutoMapper;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.RequestTypeRepo;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Services.Membership.RequestTypeServ
{
    public class RequestTypeService : IRequestTypeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IRequestTypeRepository requestTypeRepository;
        public RequestTypeService(IUnitOfWork unitOfWork, IMapper mapper, IRequestTypeRepository requestTypeRepository)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.requestTypeRepository = requestTypeRepository;
        }
        public async Task<Response<RequestTypeDto>> AddRequestType(RequestTypeDto requestTypeDto)
        {
            try
            {
                var type = mapper.Map<RequestType>(requestTypeDto);
                await unitOfWork.Repository<RequestType>().Add(type);
                await unitOfWork.Save();
                requestTypeDto = mapper.Map<RequestTypeDto>(type);
                return new ResponseHandler().Success(requestTypeDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RequestTypeDto>(ex.Message);
            }
        }

        public async Task<Response<RequestTypeDto>> DeleteRequestTypeById(int Id)
        {
            try
            {
                var type = await unitOfWork.Repository<RequestType>().GetByIdAsync(Id);
                unitOfWork.Repository<RequestType>().Delete(type);
                await unitOfWork.Save();
                var result = mapper.Map<RequestTypeDto>(type);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RequestTypeDto>(ex.Message);
            }
        }

        public async Task<Response<List<RequestTypeDto>>> GetAllRequestTypes()
        {
            try
            {
                var types = await unitOfWork.Repository<RequestType>().GetAllAsync();
                var result = mapper.Map<List<RequestTypeDto>>(types);
                return new ResponseHandler().Success(result); 
            }
            catch(Exception ex)
            {
                return new ResponseHandler().BadRequest<List<RequestTypeDto>>(ex.Message);
            }
        }

        public async Task<Response<RequestTypeDto>> GetRequestTypeById(int Id)
        {
            try
            {
                var type = await unitOfWork.Repository<RequestType>().GetByIdAsync(Id);
                var result = mapper.Map<RequestTypeDto>(type);
                return new ResponseHandler().Success(result);
            }
            catch(Exception ex)
            {
                return new ResponseHandler().BadRequest<RequestTypeDto>(ex.Message);
            }
        }

        public async Task<Response<RequestTypeDto>> UpdateRequestType(RequestTypeDto requestTypeDto)
        {
            try
            {
                var type = mapper.Map<RequestType>(requestTypeDto);
                unitOfWork.Repository<RequestType>().Update(type);
                await unitOfWork.Save();
                return new ResponseHandler().Success(requestTypeDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RequestTypeDto>(ex.Message);
            }
        }

        public async Task<Response<List<RequestTypeDto>>> GetAllTypeExceptNew()
        {
            try
            {
                var types = await requestTypeRepository.GetAllTypeExceptNew();
                var result = mapper.Map<List<RequestTypeDto>>(types);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<RequestTypeDto>>(ex.Message);
            }
        }
    }
}
