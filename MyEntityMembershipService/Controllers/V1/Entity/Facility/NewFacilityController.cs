using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.FacilityDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.FacilityService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Facility
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class NewFacilityController : ControllerBase
    {
        private readonly IFacilityService _facilityService;

        public NewFacilityController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        [HttpPost("AddFacilityAsync")]
        public async Task<IResult> AddFacilityAsync([FromForm] AddFacilityDto facilityDto)

            => (await _facilityService.AddFacilityAsync(facilityDto)).GetResponse();
        [HttpPost("AddFacilityOperatingHours")]
        public async Task<IResult> AddFacilityOperatingHours([FromBody] FacilityOperatingHoursDto facilityOperatingHoursDto)

            => (await _facilityService.AddFacilityOperatingHoursAsync(facilityOperatingHoursDto)).GetResponse();
        [HttpGet("GetFaciityCardList")]
        public async Task<IResult> GetFaciityCardList(int pageNumber, int branchID)

            => (await _facilityService.GetFaciityCardList(pageNumber, branchID)).GetResponse();
        [HttpGet("GetAllFacilities")]
        public async Task<IResult> GetAllFacilities()

            => (await _facilityService.GetAllFacilities()).GetResponse();
        [HttpGet("GetFacilityDetailsByIdAsync/{facilityID}")]
        public async Task<IResult> GetFacilityDetailsByIdAsync(int facilityID)

            => (await _facilityService.GetFacilityDetailsByIdAsync(facilityID)).GetResponse();
        [HttpDelete("RemoveFacilityAsync/{facilityID}")]
        public async Task<IResult> RemoveFacilityAsync(int facilityID)

            => (await _facilityService.RemoveFacilityAsync(facilityID)).GetResponse();
        [HttpDelete("RemoveFacilityMediaAsync")]
        public async Task<IResult> RemoveFacilityMediaAsync(int facilityID, int mediaId)
        {
            var result = await _facilityService.RemoveFacilityMediaAsync(facilityID, mediaId);
            return result.GetResponse();
        }
        [HttpPost("EditFacilityAsync")]
        public async Task<IResult> EditFacilityAsync([FromForm] EditFacilityDto facilityDto)

            => (await _facilityService.EditFacilityAsync(facilityDto)).GetResponse();
        [HttpGet("GetFacilityEditDetailsByIdAsync/{facilityID}")]
        public async Task<IResult> GetFacilityEditDetailsByIdAsync(int facilityID)

            => (await _facilityService.GetFacilityEditDetailsByIdAsync(facilityID)).GetResponse();
        [HttpPost("AddFacilityNewMedia")]
        public async Task<IResult> AddFacilityNewMedia([FromForm] List<IFormFile> newMedia, int facilityID)
        {
            var result = await _facilityService.AddFacilityNewMedia(newMedia, facilityID);
            return result.GetResponse();
        }
        [HttpPut("UpdateFacilityOperatingHoursAsync")]
        public async Task<IResult> UpdateFacilityOperatingHoursAsync(EditFacilityOperatingHoursDto facilityDto)
        {
            var result = await _facilityService.UpdateFacilityOperatingHoursAsync(facilityDto);
            return result.GetResponse();
        }
        [HttpGet("GetMobileFacilityVenues/{facilityID}")]
        public async Task<IResult> GetMobileFacilityVenues(int facilityID)
        {
            var result = await _facilityService.GetMobileFacilityVenues(facilityID);
            return result.GetResponse();
        }
        [HttpGet("GetMobileFacilityDetailsAsync/{facilityID}")]
        public async Task<IResult> GetMobileFacilityDetailsAsync(int facilityID)
        {
            var result = await _facilityService.GetMobileFacilityDetailsAsync(facilityID);
            return result.GetResponse();
        }
        [HttpGet("GetFacilitiesByBranchID/{branchID}")]
        public async Task<IResult> GetFacilitiesByBranchID(int branchID)
        {
            var result = await _facilityService.GetFacilitiesByBranchID(branchID);
            return result.GetResponse();
        }


        [HttpGet("Facilities/{branchId}")]
        public async Task<IResult> GetFacilitiesByBranch(int branchId , [FromQuery] NewFacilityDashboardSearchDto dto )
        {
            var result = await _facilityService.GetFacilitiesByBranch(branchId, dto);
            return result.GetResponse();
        }
    }
}
