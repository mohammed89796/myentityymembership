using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.FieldsDto;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.FieldServ;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class FieldsController : ControllerBase
    {
        private readonly IFieldService fieldService;

        public FieldsController(IFieldService fieldService)
        {
            this.fieldService = fieldService;
        }
        [HttpGet("GetAllFields")]
        public async Task<IResult> GetAllFields()
        {
            var result = await fieldService.GetAllFields();
            return result.GetResponse();
        }



        [HttpGet("GetFieldById/{id:int}")]
        public async Task<IResult> GetFieldById([FromRoute] int id)
        {
            var result = await fieldService.GetFieldById(id);
            return result.GetResponse();
        }


        [HttpPost("CreateField")]
        public async Task<IResult> CreateField([FromBody] CreateFieldDto dto)
        {
            var result = await fieldService.CreateField(dto);
            return result.GetResponse();
        }


        [HttpPut("UpdateField")]
        public async Task<IResult> UpdateField([FromBody] UpdateFieldDto dto)
        {
            var result = await fieldService.UpdateField(dto);
            return result.GetResponse();
        }

       
        [HttpDelete("DeleteField/{id:int}")]   
        public async Task<IResult> DeleteField([FromRoute] int id)
        {
            var result = await fieldService.DeleteField(id);
            return result.GetResponse();
        }
    }
}
