using AutoMapper;
using MyEntityMembershipService.Data.Membership;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.FormDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.MemberTypeRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Membership.MemberTypesServ
{
    public class MemberTypeService : IMemberTypeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMemberTypeRepo memberTypeRepo;

        public MemberTypeService(IUnitOfWork unitOfWork, IMapper mapper, IMemberTypeRepo memberTypeRepo)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.memberTypeRepo = memberTypeRepo;
        }

        public async Task<Response<MemberTypeDto>> AddMemberType(MemberTypeDto memberType)
        {
            try
            {
                var type = mapper.Map<MemberType>(memberType);
                await unitOfWork.Repository<MemberType>().Add(type);
                await unitOfWork.Save();
                memberType = mapper.Map<MemberTypeDto>(type);
                return new ResponseHandler().Success(memberType);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MemberTypeDto>(ex.Message);
            }
        }

        public async Task<Response<List<MemberTypeDto>>> GetAllTypes( string? Name , int? requestTypeId)
        {
            try
            {
                Expression<Func<MemberRequestType, bool>> filter = mrt =>
                    (!requestTypeId.HasValue || mrt.RequestTypeId == requestTypeId.Value)
                    &&
                    (string.IsNullOrEmpty(Name) || mrt.MemberType.Name.ToLower().Contains(Name.Trim().ToLower()));


                Expression<Func<MemberRequestType, MemberTypeDto>> selector = mrt => new MemberTypeDto
                {
                    Id = mrt.MemberType.Id,
                    Name = mrt.MemberType.Name,
                    ArabicName = mrt.MemberType.ArabicName
                };

                var memberTypeSpec = new GeneralSpecifications<MemberRequestType>(filter);
                var memberTypes = await unitOfWork.Repository<MemberRequestType>().GetAllWithSpecificationsAndProjection(memberTypeSpec, selector);


                Log.Information("Retrieved {Count} member types from the database.", memberTypes.Count);

                return new ResponseHandler().Success(memberTypes);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MemberTypeDto>>(ex.Message);
            }
        }
        public async Task<Response<MemberTypeDto>> GeTypeById(int id)
        {
            try
            {
                var memberTypes = await unitOfWork.Repository<MemberType>().GetByIdAsync(id);
                var types = mapper.Map<MemberTypeDto>(memberTypes);
                return new ResponseHandler().Success(types);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<MemberTypeDto>(ex.Message);
            }
        }
        public async Task<Response<List<MemberTypeDto>>> GetAllTypeExceptMain()
        {
            try
            {
                var memberTypes = await memberTypeRepo.GetAllTypeExceptMain();

                if (memberTypes == null || !memberTypes.Any())
                {
                    return new ResponseHandler().NotFound<List<MemberTypeDto>>("No member types found.");
                }

                // Use AutoMapper to map entities to DTOs
                var memberTypeDtos = mapper.Map<List<MemberTypeDto>>(memberTypes);

                return new ResponseHandler().Success(memberTypeDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<MemberTypeDto>>(ex.Message);
            }
        }

    }
}
