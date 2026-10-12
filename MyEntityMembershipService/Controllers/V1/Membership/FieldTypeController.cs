using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FieldTypeServ;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FieldTypeController : ControllerBase
    {
        private readonly IFieldTypeService fieldTypeService;

        public FieldTypeController(IFieldTypeService fieldTypeService)
        {
            this.fieldTypeService = fieldTypeService;
        }
        [HttpGet("GetAllTypes")]
        public async Task<IResult> GetAllTypes()
        {
            var result = await fieldTypeService.GetAllTypes();
            return result.GetResponse();
        }
        [HttpPost("AddFieldType")]
        public async Task<IResult> AddFieldType(FieldTypeDto fieldTypeDto)
        {
            var result = await fieldTypeService.AddFieldType(fieldTypeDto);
            return result.GetResponse();
        }
    }
}
