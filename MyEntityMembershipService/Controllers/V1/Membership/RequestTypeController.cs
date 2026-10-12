using Asp.Versioning;
using AutoMapper;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;
using MyEntityMembershipService.Services.Membership.RequestTypeServ;
using Microsoft.AspNetCore.Authorization;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class RequestTypeController : ControllerBase
    {
        private readonly IRequestTypeService requestTypeService;

        public RequestTypeController(IRequestTypeService requestTypeService)
        {
            this.requestTypeService = requestTypeService;
        }
        [HttpGet("GetAllRequestTypes")]
        public async Task<IResult> GetAllRequestTypes()
        {
            var result = await requestTypeService.GetAllRequestTypes();
            return result.GetResponse();
        }
        [HttpGet("GetRequestTypeById")]
        public async Task<IResult> GetRequestTypeById(int Id)
        {
            var result = await requestTypeService.GetRequestTypeById(Id);
            return result.GetResponse();
        }
        [HttpPut("UpdateRequestType")]
        public async Task<IResult> UpdateRequestType(RequestTypeDto requestTypeDto)
        {
            var result = await requestTypeService.UpdateRequestType(requestTypeDto);
            return result.GetResponse();
        }
        [HttpPost("AddRequestType")]
        public async Task<IResult> AddRequestType(RequestTypeDto requestTypeDto)
        {
            var result = await requestTypeService.AddRequestType(requestTypeDto);
            return result.GetResponse();
        }
        [HttpDelete("DeleteRequestType")]
        public async Task<IResult> DeleteRequestType(int Id)
        {
            var result = await requestTypeService.DeleteRequestTypeById(Id);
            return result.GetResponse();
        }
        [HttpGet("GetAllTypeExceptNew")]
        public async Task<IResult> GetAllTypeExceptNew()
        {
            var result = await requestTypeService.GetAllTypeExceptNew();
            return result.GetResponse();
        }
    }
}
