using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs.Entity.FacilityDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FacilityController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly EntityMembershipDbContext _Context;
        private readonly IConfiguration _configuration;
        public FacilityController(IUnitOfWork unitOfWork, IMapper mapper, EntityMembershipDbContext Context, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _Context = Context;
            _configuration = configuration;
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var facilities = _Context.Facilities.Include(fc => fc.Branch).Include(fc => fc.FacilityOperatingHours).Include(fc => fc.FacilityAccessibilityFeatures).Include(fc => fc.FacilityAmenities).Include(fc => fc.FacilitySportsAccommodated).Include(fc => fc.FacilityType).Include(fc => fc.Venues).ThenInclude(v => v.VenueOperatingHours).ToList();

                var tasks = facilities.Select(async facility =>
                {

                    facility.Image = await DocumentSetting.GetPreSignedUrlAsync(facility.Image);

                    if (facility.Venues != null)
                    {
                        var venueTasks = facility.Venues.Select(async venue =>
                         {
                             venue.Image = await DocumentSetting.GetPreSignedUrlAsync(venue.Image);
                         });
                        await Task.WhenAll(venueTasks);
                    }

                    if (facility.Branch != null)
                    {
                        facility.Branch.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(facility.Branch.BranchImageURL);
                    }
                });
                await Task.WhenAll(tasks);

                return Ok(facilities);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet("GetByPagination")]
        public async Task<IActionResult> GetByPagination(int pageNo = 1, int pageSize = 10)
        {
            try
            {
                var GetFacilityPaginationDto = new GetFacilityPaginationDto();
                var CountOflistOfFacilities = (int)Math.Ceiling((double)_Context.Facilities.Count() / pageSize);
                var GetlistOfFacilities = _Context.Facilities.Skip((pageNo - 1) * pageSize).Take(pageSize).Include(fc => fc.Branch).Include(fc => fc.FacilityOperatingHours).Include(fc => fc.FacilityAccessibilityFeatures).Include(fc => fc.FacilityAmenities).Include(fc => fc.FacilitySportsAccommodated).Include(fc => fc.FacilityType).Include(fc => fc.Venues).ThenInclude(v => v.VenueOperatingHours).ToList();
                GetFacilityPaginationDto.totalPages = CountOflistOfFacilities;
                GetFacilityPaginationDto.currentPage = pageNo;
                GetFacilityPaginationDto.facilities = GetlistOfFacilities;

                var tasks = GetlistOfFacilities.Select(async facility =>
                {

                    facility.Image = await DocumentSetting.GetPreSignedUrlAsync(facility.Image);
                    if (facility.Venues != null)
                    {
                        var venueTasks = facility.Venues.Select(async venue =>
                         {
                             venue.Image = await DocumentSetting.GetPreSignedUrlAsync(venue.Image);
                         });
                        await Task.WhenAll(venueTasks);
                    }
                    if (facility.Branch != null)
                    {
                        facility.Branch.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(facility.Branch.BranchImageURL);
                    }
                });
                await Task.WhenAll(tasks);
                return Ok(GetFacilityPaginationDto);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var Facility = await _Context.Facilities.Include(or => or.Branch).Where(or => or.Id == id).Include(fc => fc.FacilityOperatingHours).Include(fc => fc.FacilityAccessibilityFeatures).Include(fc => fc.FacilityAmenities).Include(fc => fc.FacilitySportsAccommodated).Include(fc => fc.FacilityType).Include(fc => fc.Venues).ThenInclude(v => v.VenueOperatingHours).FirstOrDefaultAsync();

                if (Facility is null)
                {
                    return BadRequest("Facility Not Found");
                }
                Facility.Image = await DocumentSetting.GetPreSignedUrlAsync(Facility.Image);
                if (Facility.Venues != null)
                {
                    var venueTasks = Facility.Venues.Select(async venue =>
                     {
                         venue.Image = await DocumentSetting.GetPreSignedUrlAsync(venue.Image);
                     });
                    await Task.WhenAll(venueTasks);
                }
                if (Facility.Branch != null)
                {
                    Facility.Branch.BranchImageURL = await DocumentSetting.GetPreSignedUrlAsync(Facility.Branch.BranchImageURL);
                }

                return Ok(Facility);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        // [HttpPost("CreateFacility")]
        // public async Task<IActionResult> CreateFacility([FromForm]AddFacilityDto model)
        // {
        //     try
        //     {
        //         if (ModelState.IsValid)
        //         {
        //             if (_Context.Facilities.Any(or => or.name == model.name))
        //             {
        //                 return BadRequest("This Name Already Exist");
        //             }
        //             var facility = _mapper.Map<Facility>(model);

        //             facility.Image = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", model.Image);

        //             var FacilityID = await _Context.Facilities.AddAsync(facility);
        //             await _Context.SaveChangesAsync();
        //             if (model.FacilityAccessibilityFeaturesId != null)
        //             {
        //                 foreach (var item in model.FacilityAccessibilityFeaturesId)
        //                 {
        //                     var FacilityAccessibilityFeatures = new FacilityAccessibilityFeatures
        //                     {
        //                         FacilityId = FacilityID.Entity.Id,
        //                         AccessibilityFeaturesId = item
        //                     };
        //                     await _unitOfWork.Repository<FacilityAccessibilityFeatures>().Add(FacilityAccessibilityFeatures);
        //                 }

        //             }
        //             if (model.FacilityAmenitiesId != null)
        //             {
        //                 foreach (var item in model.FacilityAmenitiesId)
        //                 {
        //                     var FacilityAmenities = new FacilityAmenities
        //                     {
        //                         FacilityId = FacilityID.Entity.Id,
        //                         StadFacilityId = item
        //                     };
        //                     await _unitOfWork.Repository<FacilityAmenities>().Add(FacilityAmenities);
        //                 }
        //             }
        //             if (model.FacilitySportsAccommodatedId != null)
        //             {
        //                 foreach (var item in model.FacilitySportsAccommodatedId)
        //                 {
        //                     var FacilitySportsAccommodated = new FacilitySportsAccommodated
        //                     {
        //                         FacilityId = FacilityID.Entity.Id,
        //                         SportsAccommodatedId = item
        //                     };
        //                     await _unitOfWork.Repository<FacilitySportsAccommodated>().Add(FacilitySportsAccommodated);
        //                 }
        //             }
        //             await _unitOfWork.Save();
        //             return Ok(FacilityID.Entity.Id);
        //         }
        //         else
        //         {
        //             return BadRequest(ModelState);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest("Something went wrong");
        //     }
        // }
        [HttpPut("UpdateFacility")]
        public async Task<IActionResult> UpdateFacility([FromForm] UpdateFacilityDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var facility = _mapper.Map<Data.Entity.Entities.Facility>(model);
                    _unitOfWork.Repository<Data.Entity.Entities.Facility>().Update(facility);
                    await _unitOfWork.Save();
                    return Ok("Facility Updated Successfully");
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Facility = await _unitOfWork.Repository<Data.Entity.Entities.Facility>().GetByIdAsync(id);
                if (Facility is null)
                {
                    return BadRequest("Facility Not Found");
                }
                _unitOfWork.Repository<Data.Entity.Entities.Facility>().Delete(Facility);
                await _unitOfWork.Save();
                return Ok("Facility Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpPost("SetOpreatingHours")]
        public async Task<IActionResult> SetOpreatingHours(SetFacilityOperatingDto model)
        {
            try
            {
                if (model is null)
                {
                    return BadRequest("Invaild Object");
                }
                var OpreatingHoursMapping = _mapper.Map<FacilityOperatingHours>(model);
                await _unitOfWork.Repository<FacilityOperatingHours>().Add(OpreatingHoursMapping);
                await _unitOfWork.Save();
                return Ok(model);
            }
            catch (Exception ex)
            {

                return BadRequest("Something went wrong");
            }

        }
        [HttpPut("UpdateOpreatingHours")]
        public async Task<IActionResult> UpdateOpreatingHours(UpdateFacilityOperatingHoursDto model)
        {
            try
            {
                if (model is null)
                {
                    return BadRequest("Invaild Object");
                }
                //   var OpreatingHours = await _unitOfWork.Repository<FacilityOperatingHours>().GetByIdAsync(model.Id);
                var OpreatingHoursMapping = _mapper.Map<FacilityOperatingHours>(model);
                _unitOfWork.Repository<FacilityOperatingHours>().Update(OpreatingHoursMapping);
                await _unitOfWork.Save();
                return Ok("Opreating Hours Updated Successfully");
            }
            catch (Exception ex)
            {

                return BadRequest("Something went wrong");
            }

        }
    }
}
