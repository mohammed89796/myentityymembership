
using MyEntityMembershipService.DTOs.Entity.SystemDtos.FedrationBranches;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.System.FedrationBranchesRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Entity.System.EntityBranchService
{
    public class FedrationBranchService : IFedrationBranchService
    {
        private readonly IFedrationBranchesRepo _fedrationBranchesRepo;
        private readonly IUnitOfWork _unitOfWork;


        public FedrationBranchService(IUnitOfWork unitOfWork, IFedrationBranchesRepo fedrationBranchesRepo)
        {
            _unitOfWork = unitOfWork;
            _fedrationBranchesRepo = fedrationBranchesRepo;
        }


        public async Task<Response<bool>> CreateFedrationBranch(string enName, string arName, int TenantId)
        {
            try
            {

                var fedrationBranch = new FedrationBranch
                {
                    TenantId = TenantId,
                    Name = enName,
                    ArabicName = arName,
                    CreationDate = DateTime.UtcNow
                };


                await _unitOfWork.Repository<FedrationBranch>().Add(fedrationBranch);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");

            }
        }

        public async Task<Response<List<FedrationBranchesViewModel>>> GetBranchDropDown(int tenantId, int languageType)
        {
            try
            {
                var result = await _fedrationBranchesRepo.GetFedBranchesDropDown(tenantId, languageType);

                if (result == null || !result.Any())
                {
                    return new ResponseHandler().Success(new List<FedrationBranchesViewModel>(), "No branches found.");
                }

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<FedrationBranchesViewModel>>("Something went wrong");
            }
        }

    }

}
