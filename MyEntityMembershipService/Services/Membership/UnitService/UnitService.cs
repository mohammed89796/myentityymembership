using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.UnitDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.UnitRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using System.Security.Cryptography.Xml;

namespace MyEntityMembershipService.Services.Membership.UnitService
{
    public class UnitService : IUnitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public UnitService(IUnitOfWork unitOfWork,IUnitRepository unitRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;
            _mapper = mapper;
        }
        public async Task<Response<List<UnitDto>>> GetAllAsync()
        {
            try
            {
                //var entites = await _unitOfWork.Repository<Unit>()
                //.GetAllAsync();

                var entities = await _unitRepository.GetAllWithUnitTypeAsync();
                if (entities is null)
                    return new ResponseHandler().NotFound<List<UnitDto>>("No Units is found.");

                var dtos = _mapper.Map<List<UnitDto>>(entities);
                return new ResponseHandler().Success(dtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<UnitDto>>(ex.InnerException?.Message ?? ex.Message);
            }
            
        }

        public async Task<Response<UnitDto>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitRepository.GetByIdWithUnitTypeAsync(id);

                if (entity is null)
                    return new ResponseHandler().NotFound<UnitDto>($"Unit with ID = {id} is not found.");

                var dto = _mapper.Map<UnitDto>(entity);
                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<UnitDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<UnitDto>> GetByNameAsync(string name)
        {
            try
            {
                var entity = await _unitRepository.GetByNameWithUnitTypeAsync(name);

                if (entity is null)
                    return new ResponseHandler().NotFound<UnitDto>($"Unit with name = {name} is not found.");

                var dto = _mapper.Map<UnitDto>(entity);
                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<UnitDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<UnitDto>>> GetByUnitTypeIdAsync(int unitTypeId)
        {
            try
            {
                var entities = await _unitRepository.GetByUnitTypeIdWithTypeNameAsync(unitTypeId);

                if (entities is null)
                    return new ResponseHandler().NotFound<List<UnitDto>>($"There is no Units with UnitTypeId = {unitTypeId}");

                var dtos = _mapper.Map<List<UnitDto>>(entities);
                return new ResponseHandler().Success(dtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<UnitDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<UnitDto>> AddAsync(AddUnitDto dto)
        {
            try
            {
                var entity = _mapper.Map<Unit>(dto);

                await _unitOfWork.Repository<Unit>().Add(entity);
                await _unitOfWork.Save();

                var returnedDto = _mapper.Map<UnitDto>(entity);

                return new ResponseHandler().Success(returnedDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<UnitDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        

        public async Task<Response<UnitDto>> UpdateAsync(UnitDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Unit>().GetByIdAsync(dto.Id);

                if (entity is null)
                    return new ResponseHandler().NotFound<UnitDto>($"Unit with Id = {dto.Id} is not found.");

                _unitOfWork.Repository<Unit>().Update(entity);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<UnitDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> Delete(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Unit>().GetByIdAsync(id);

                if (entity is null)
                    return new ResponseHandler().NotFound<bool>($"Unit with id = {id} is not found.");

                _unitOfWork.Repository<Unit>().Delete(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
