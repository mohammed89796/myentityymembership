

using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.FacilityDtos;
using MyEntityMembershipService.Repositories.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Services.Entity.FacilityService
{
    public interface IFacilityService
    {
        Task<Response<AddFacilityDto>> AddFacilityAsync(AddFacilityDto facilityDto);
        Task<Response<bool>> AddFacilityOperatingHoursAsync(FacilityOperatingHoursDto? operatingHoursDto);
        Task<Response<FacilityListDetailsDto>> GetFaciityCardList(int pageNumber, int branchID);
        Task<Response<FacilityDetailsDto>> GetFacilityDetailsByIdAsync(int facilityID);
        Task<Response<bool>> RemoveFacilityAsync(int facilityId);
        Task<Response<List<FacilityDetailsDto>>> GetAllFacilities();
        Task<Response<bool>> RemoveFacilityMediaAsync(int facilityID, int mediaId);
        Task<Response<EditFacilityDto>> EditFacilityAsync(EditFacilityDto facilityDto);
        Task<Response<EditFacilityDetailsDto>> GetFacilityEditDetailsByIdAsync(int facilityID);
        Task<Response<bool>> AddFacilityNewMedia(List<IFormFile> newMedia, int facilityID);
        Task<Response<bool>> UpdateFacilityOperatingHoursAsync(EditFacilityOperatingHoursDto facilityDto);
        Task<Response<List<MobileFacilityVenuesDto>>> GetMobileFacilityVenues(int facilityID);
        Task<Response<GetFacilityMobileDetailsDto>> GetMobileFacilityDetailsAsync(int facilityID);
        Task<Response<List<NewFacilityDashboardDto>>> GetFacilitiesByBranchID(int branchID);
        Task<Response<PaginationToReturnDto<NewFacilityDashboardDto>>> GetFacilitiesByBranch(int branchId, NewFacilityDashboardSearchDto dto);
        Task<int> GetTotalNumberOfFacilitiesByTenantId(int tenantId);
    }
}
