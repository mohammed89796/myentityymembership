using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.MemberTypesServ;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class MemberTypeController : ControllerBase
    {
        private readonly IMemberTypeService memberTypeService;

        public MemberTypeController(IMemberTypeService memberTypeService)
        {
            this.memberTypeService = memberTypeService;
        }
        [HttpGet("GetAllTypes")]
        public async Task<IResult> GetAllTypes(string? Name, int? requestTypeId)
        {
            var result = await memberTypeService.GetAllTypes(Name, requestTypeId);
            return result.GetResponse();
        }
        [HttpPost("AddMemberType")]
        public async Task<IResult> AddMemberType(MemberTypeDto memberTypeDto)
        {
            var result = await memberTypeService.AddMemberType(memberTypeDto);
            return result.GetResponse();
        }
        [HttpGet("GetMemberTypeById")]
        public async Task<IResult> GetMemberTypeById(int id)
        {
            var result = await memberTypeService.GeTypeById(id);
            return result.GetResponse();
        }

        [HttpGet("GetAllTypeExceptMain")]
        public async Task<IResult> GetAllTypeExceptMain()
        {
            var result = await memberTypeService.GetAllTypeExceptMain();
            return result.GetResponse();
        }
    }
}
