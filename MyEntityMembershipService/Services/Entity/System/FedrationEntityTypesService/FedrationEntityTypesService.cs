using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.SystemDtos.FedrationEntityTypes;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.System.FedrationEntityTypesRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Entity.System.FedrationEntityTypesService
{
    public class FedrationEntityTypesService : IFedrationEntityTypesService
    {
        private readonly IFedrationEntityTypesRepo _fedrationEntityTypesRepo;
        private readonly IUnitOfWork _unitOfWork;


        public FedrationEntityTypesService(IUnitOfWork unitOfWork, IFedrationEntityTypesRepo fedrationEntityTypesRepo)
        {
            _unitOfWork = unitOfWork;
            _fedrationEntityTypesRepo = fedrationEntityTypesRepo;
        }


        public async Task<Response<bool>> CreateFedrationEntityType(string name)
        {
            try
            {

                var fedrationEntityType = new FedrationEntityType
                {
                    Name = name,
                    CreationDate = DateTime.UtcNow
                };


                await _unitOfWork.Repository<FedrationEntityType>().Add(fedrationEntityType);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");

            }
        }

        public async Task<Response<List<FedrationEntityTypesViewModel>>> GetFedEntityTypes()
        {
            try
            {
                var result = await _fedrationEntityTypesRepo.GetFedEntityTypes();

                if (result == null || !result.Any())
                {
                    return new ResponseHandler().Success(new List<FedrationEntityTypesViewModel>(), "No branches found.");
                }

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<FedrationEntityTypesViewModel>>("Something went wrong");
            }
        }

    }

}
