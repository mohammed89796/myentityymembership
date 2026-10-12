using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.FormDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Membership.FieldTypeServ
{
    public class FieldTypeService : IFieldTypeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public FieldTypeService( IUnitOfWork unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Response<FieldTypeDto>> AddFieldType(FieldTypeDto fieldType)
        {
            try
            {
                var type = mapper.Map<FieldType>(fieldType);
                await unitOfWork.Repository<FieldType>().Add(type);
                await unitOfWork.Save();
                fieldType = mapper.Map<FieldTypeDto>(type);
                return new ResponseHandler().Success(fieldType);
            }
            catch(Exception ex)
            {
                return new ResponseHandler().BadRequest<FieldTypeDto>(ex.Message);
            }
        }

        public async Task<Response<List<FieldTypeDto>>> GetAllTypes()
        {
            try
            {
                var fieldTypes = await unitOfWork.Repository<FieldType>().GetAllAsync();
                var types = mapper.Map<List<FieldTypeDto>>(fieldTypes);
                return new ResponseHandler().Success(types);
            }
            catch( Exception ex )
            {
                return new ResponseHandler().BadRequest<List<FieldTypeDto>>(ex.Message);
            }
        }
    }
}
