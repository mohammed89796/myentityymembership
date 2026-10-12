using Asp.Versioning;

using MyEntityMembershipService.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.Services.Membership.MembershipServ;
using Microsoft.AspNetCore.Authorization;
using MyEntityMembershipService.DTOs.Membership.Swimming;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }
        [HttpPost("CreateMembership")]
        public async Task<IResult> CreateMembership(MembershipDto membershipDto)
        {
            var result = await membershipService.CreateMembership(membershipDto);
            return result.GetResponse();
        }
        [HttpPost("CreateSumMemberMembership")]
        public async Task<IResult> CreateSumMemberMembership(subMemberMembershipDto subMemberMembershipDto)
        {
            var result = await membershipService.CreateSubMemberMembership(subMemberMembershipDto);
            return result.GetResponse();
        }

        [HttpGet("GeMembershipsByPackageId")]
        public async Task<IResult> GeMembershipsByPackageId(int packageId)
        {
            var result = await membershipService.GeMembershipsByPackageId(packageId);
            return result.GetResponse();
        }
        [HttpGet("GeMembershipsByTenantId/{tenantId}")]
        public async Task<IResult> GeMembershipsByTenantId(int tenantId)
        {
            var result = await membershipService.GeMembershipsByTenantId(tenantId);
            return result.GetResponse();
        }

        [HttpGet("GetMembershipsWithPackage/{year}")]
        public async Task<IResult> GetMembershipsWithPackage(int year)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await membershipService.GetMembershipsGroupedByPackageNameAsync(year,tenantId);
            return result.GetResponse();
        }
        [HttpGet("GetMembershipGrowthRateSummary/{year}")]
        public async Task<IResult> GetMembershipGrowthRateSummary(int year)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await membershipService.MembershipsGrowthRatePerPackage(year, tenantId);
            return result.GetResponse();
        }

        [HttpGet("GetMembershipsSegmentationByPackage/{year}/{tenantId}")]
        public async Task<IResult> GetMembershipsSegmentationByPackage(int year, int tenantId)
        {
            var result = await membershipService.GetMembershipsSegmentationByPackageAsync(year, tenantId);
            return result.GetResponse();
        }

        [HttpGet("GetTenantMembers")]
        public async Task<IResult> GetTenantMembers([FromQuery]GetTenantMembersParamDto parameters)
        {
            var result = await membershipService.GetTenantMembers(parameters);
            return result.GetResponse();
        }
        [HttpGet("GetFederationMembers")]
        public async Task<IResult> GetFederationMembers([FromQuery] GetFederationMembersParamDto parameters)
        {
            var result = await membershipService.GetFederationMembers(parameters);
            return result.GetResponse();
        }
        [HttpGet("GetBelts")]
        public async Task<IResult> GetBelts()
        {
            var result = await membershipService.GetBelts();
            return result.GetResponse();
        }
        [HttpGet("GetSwimmingMembers")]
        public async Task<IResult> GetSwimmingMembers([FromQuery] GetSwimmingMembersParamDto paramDto)
        {
            paramDto.UserId = User.GetUserId();
            paramDto.IsSuperAdmin = User.IsSuperAdmin();
            paramDto.IsTenantAdmin = User.IsTenantAdmin();
            var result = await membershipService.GetSwimmingMembers(paramDto);
            return result.GetResponse();
        }

        [HttpGet("GetAllSwimmingMembers")]
        public async Task<IResult> GetAllSwimmingMembers()
        {
            var result = await membershipService.GetAllSwimmingMembers();
            return result.GetResponse();
        }

        [HttpGet("GetMembershipById")]
        public async Task<IResult> GetMembershipById(int membershipId)
        {
            var result = await membershipService.GetMembershipInjuryStatusAsync(membershipId);
            return result.GetResponse();
        }
        [HttpGet("SearchMemberships")]
        public async Task<IResult> SearchMemberships([FromQuery] string? searchTerm)
        {
            var result = await membershipService.SearchMembershipsAsync(searchTerm);
            return result.GetResponse();
        }

        [HttpGet("GetAllUserMemberships")]
        public async Task<IResult> GetAllUserMemberships([FromQuery] List<string> ChildrenIds)
        {
            var result = await membershipService.GetAllUserMembershipsAsync(ChildrenIds);
            return result.GetResponse();
        }


        [HttpPost("CreateSwimmingMembership")]
        public async Task<IResult> CreateMembership([FromForm]CreateMembershipDto membershipDto)
        {
            var result = await membershipService.CreateSwimmingMembership(membershipDto);
            return result.GetResponse();
        }
        [HttpPut("UpdateSwimmingMembership")]
        public async Task<IResult> UpdateSwimmingMembership([FromForm] UpdateMembershipDto membershipDto)
        {
            var result = await membershipService.UpdateSwimmingMembership(membershipDto);
            return result.GetResponse();
        }
        [HttpDelete("DeleteSwimmingMembership")]
        public async Task<IResult> DeleteSwimmingMembership(int membershipId)
        {
            var result = await membershipService.SoftDeleteSwimmingMembership(membershipId);
            return result.GetResponse();
        }
        [HttpDelete("UnSoftDeleteSwimmingMembership")]
        public async Task<IResult> UnSoftDeleteSwimmingMembership(int membershipId)
        {
            var result = await membershipService.UnSoftDeleteSwimmingMembership(membershipId);
            return result.GetResponse();
        }
        [HttpGet("GetMembersSportLevels")]
        public async Task<IResult> GetMembersSportLevels([FromQuery] GetMemberSportLevelsParamDto getMemberSportLevelsParamDto)
        {
            var result = await membershipService.GetMembersSportLevels(getMemberSportLevelsParamDto);
            return result.GetResponse();
        }


        [HttpGet("GetSportDistribution")]
        public async Task<IResult> GetSportDistribution([FromQuery] SportDistributionFilterDto filterDto)
        {
            var tenantId = User.GetParsedTenantId();
            var result = await membershipService.GetSportDistribution(filterDto, tenantId);
            return result.GetResponse();
        }
        [HttpGet("UserHasMainMembershipAsync")]
        public async Task<IResult> UserHasMainMembershipAsync(int tenantId)
        {
            var result = await membershipService.UserHasMainMembershipAsync(tenantId);
            return result.GetResponse();
        }


        [HttpGet("GetEntitiesByUserId")]
        public async Task<IResult> GetEntitiesByUserIdAsync()
        {
            var userId = User.GetUserId();
            var result = await membershipService.GetEntitiesByUserIdAsync(userId!);
            return result.GetResponse();
        }


        [HttpGet("member/{memberId}/sport-levels")]
        public async Task<IResult> GetAllSportLevelByUserId([FromQuery] SportLevelSearchDto searchDto,string memberId)
        {
            var result = await membershipService.GetAllSportLevelByUserId(memberId, searchDto);
            return result.GetResponse();
        }

        [HttpGet("member/{memberId}/sport-level-tenants/{sportId}")]
        public async Task<IResult> GetAllSportLevelTenantsByUserId([FromQuery] SportLevelTenantSearchDto searchDto, string memberId, int sportId)
        {
            var result = await membershipService.GetAllSportLevelTenantsByUserId(memberId, sportId, searchDto);
            return result.GetResponse();
        }

        
    }
}
