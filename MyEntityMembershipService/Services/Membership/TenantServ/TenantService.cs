

//using AutoMapper;
//using MyEntityMembershipService.Data;
//using MyEntityMembershipService.Repositories.Common;
//using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

//namespace MyEntityMembershipService.Services.Membership.TenantServ
//{
//    public class TenantAccessService : ITenantAccessService
//    {
//        private readonly IUnitOfWork unitOfWork;
//        private readonly IMapper mapper;

//        public TenantAccessService(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            this.unitOfWork = unitOfWork;
//            this.mapper = mapper;
//        }

//        public async Task<Response<List<int>>> GetAllowedTenantIdsAsync(int tenantId)
//        {
//            try
//            {
//                var tenant = mapper.Map<Tenant>(addTenantDto);
//                tenant.CreationDate = DateTime.Now;
//                await unitOfWork.Repository<Tenant>().Add(tenant);
//                await unitOfWork.Save();
//                addTenantDto = mapper.Map<AddTenantDto>(tenant);
//                return new ResponseHandler().Success<AddTenantDto>(addTenantDto);
//            }
//            catch (Exception ex)
//            {
//                return new ResponseHandler().BadRequest<AddTenantDto>(ex.Message);
//            }
//        }

//                return new ResponseHandler().Success(result.Select(t => t.Id).ToList());
//            }
//            catch (Exception ex)
//            {
//                return new ResponseHandler().BadRequest<List<AddTenantDto>>(ex.Message);
//            }
//        }

//        public async Task<Response<TenantDto>> GetTenantByIdAsync(int tenantId)
//        {
//            try
//            {
//                var tenant = await unitOfWork.Repository<Tenant>().GetByIdAsync(tenantId);

//                if (tenant is not null)
//                {

//                    var mappedTenant = mapper.Map<Tenant, TenantDto>(tenant);
//                    return new ResponseHandler().Success(mappedTenant);
//                }
//                else
//                {
//                    return new ResponseHandler().NotFound<TenantDto>("Invalid tenantId");
//                }


//            }
//            catch (Exception ex)
//            {

//                return new ResponseHandler().BadRequest<TenantDto>(ex.Message);
//            }
//        }
//    }
//}
