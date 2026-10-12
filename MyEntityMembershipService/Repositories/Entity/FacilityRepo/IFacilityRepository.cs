


using MyEntityMembershipService.DTOs.Entity.FacilityDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.FacilityRepo
{
    public interface IFacilityRepository : IGenericRepository<NewFacility>
    {
        Task<FacilityListDetailsDto> GetFacilitiesByBranchIdAsync(int pageNumber, int branchID);
        Task<FacilityDetailsDto> GetFacilityDetailsByIdAsync(int facilityId);
        Task<EditFacilityDetailsDto> GetFacilityEditDetailsByIdAsync(int facilityID);
        Task<List<MobileFacilityVenuesDto>> GetMobileFacilityVenuesAsync(int facilityID);
        Task<GetFacilityMobileDetailsDto> GetMobileFacilityDetailsAsync(int facilityID);
        Task<List<NewFacilityDashboardDto>> GetFaciliesByBranchID(int branchID);
    }
}
