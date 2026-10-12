using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.OrganizationsDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Controllers.V1.Entity
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class OrganizationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly EntityMembershipDbContext _Context;
        public OrganizationController(IUnitOfWork unitOfWork, IMapper mapper, EntityMembershipDbContext Context)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _Context = Context;
        }
        [HttpGet("Get-All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var Organizations = await _Context.Organizations.Include(or => or.Facilities).ThenInclude(fc => fc.Venues).ToListAsync();

                if (Organizations.Count > 0)
                {
                    foreach (var Organization in Organizations)
                    {
                        Organization.logo = $"{Request.Scheme}://{Request.Host.Value.ToString()}{Request.PathBase.Value.ToString()}/OrganizationLogo/{Organization.logo}";
                    }
                }
                return Ok(Organizations);
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
                var GetOrganizationPaginationDto = new GetOrganizationPaginationDto();
                var CountOflistOfOrganizations = (int)Math.Ceiling((double)_Context.Organizations.Count() / pageSize);
                var GetlistOfOrganizations = _Context.Organizations.Skip((pageNo - 1) * pageSize).Take(pageSize).Include(or => or.Facilities).ThenInclude(fc => fc.Venues).ToList();

                if (GetlistOfOrganizations.Count > 0)
                {
                    foreach (var Organization in GetlistOfOrganizations)
                    {
                        Organization.logo = $"{Request.Scheme}://{Request.Host.Value.ToString()}{Request.PathBase.Value.ToString()}/OrganizationLogo/{Organization.logo}";
                    }
                }
                GetOrganizationPaginationDto.totalPages = CountOflistOfOrganizations;
                GetOrganizationPaginationDto.currentPage = pageNo;
                GetOrganizationPaginationDto.organizations = GetlistOfOrganizations;
                return Ok(GetOrganizationPaginationDto);
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
                var Organizations = await _Context.Organizations.Include(or => or.Facilities).ThenInclude(fc => fc.Venues).Where(or => or.Id == id).FirstOrDefaultAsync();
                Organizations.logo = $"{Request.Scheme}://{Request.Host.Value.ToString()}{Request.PathBase.Value.ToString()}/OrganizationLogo/{Organizations.logo}";
                return Ok(Organizations);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpPost("CreateOrganization")]
        public async Task<IActionResult> CreateOrganization([FromForm] AddOrganizationDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_Context.Organizations.Any(or => or.name == model.name))
                    {
                        return BadRequest("This Name Already Exist");
                    }
                    if (_Context.Organizations.Any(or => or.email == model.email))
                    {
                        return BadRequest("This Email Already Exist");
                    }
                    if (_Context.Organizations.Any(or => or.phoneNumber == model.phoneNumber))
                    {
                        return BadRequest("This Phone Number Already Exist");
                    }
                    var organization = new OrganizationEntity
                    {
                        name = model.name,
                        email = model.email,
                        phoneNumber = model.phoneNumber,
                        description = model.description,
                        websiteUrl = model.websiteUrl,
                        logo = model.logo != null ? DocumentSetting.UploadImageWithSpecificFolder(model.logo, "OrganizationLogo") : null,
                    };
                    _unitOfWork.Repository<OrganizationEntity>().Add(organization);
                    await _unitOfWork.Save();
                    return Ok("Organization Saved Done");
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
        [HttpPut("UpdateOrganization")]
        public async Task<IActionResult> UpdateOrganization([FromForm] UpdateOrganizationDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var organization = await _unitOfWork.Repository<OrganizationEntity>().GetByIdAsync(model.Id);
                    // organization.Id = model.Id;
                    organization.name = model.name;
                    organization.email = model.email;
                    organization.phoneNumber = model.phoneNumber;
                    organization.description = model.description != null ? model.description : organization.description;
                    organization.websiteUrl = model.websiteUrl != null ? model.websiteUrl : organization.websiteUrl;
                    organization.logo = model.logo != null ? DocumentSetting.UploadImageWithSpecificFolder(model.logo, "OrganizationLogo") : organization.logo;

                    _unitOfWork.Repository<OrganizationEntity>().Update(organization);
                    await _unitOfWork.Save();
                    return Ok("Organization Updated Successfully");
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
                var Organizations = await _unitOfWork.Repository<OrganizationEntity>().GetByIdAsync(id);
                if (Organizations is null)
                {
                    return BadRequest("Organization Not Found");
                }
                _unitOfWork.Repository<OrganizationEntity>().Delete(Organizations);
                await _unitOfWork.Save();
                return Ok("Organization Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}
