using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.UnitDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.UnitService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class UnitController : ControllerBase
    {
        private readonly IUnitService _service;

        public UnitController(IUnitService service)
            => _service = service;

        [HttpGet("GetAllUnits")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.GetResponse();
        }

        [HttpGet("GetUnitById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.GetResponse();
        }

        [HttpGet("GetUnitByName/{name:alpha}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> GetByName(string name)
        {
            var result = await _service.GetByNameAsync(name);
            return result.GetResponse();
        }

        [HttpGet("GetByUnitTypeId/{unitTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> GetByUnitTypeId(int unitTypeId)
        {
            var result = await _service.GetByUnitTypeIdAsync(unitTypeId);
            return result.GetResponse();
        }

        [HttpPost("AddUnit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> Add(AddUnitDto dto)
        {
            var result = await _service.AddAsync(dto);
            return result.GetResponse();
        }

        //[HttpPut("UpdateUnit")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IResult> Update(UnitDto dto)
        //{
        //    var result = await _service.UpdateAsync(dto);
        //    return result.GetResponse();
        //}

        [HttpDelete("RemoveUnit/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IResult> Remove(int id)
        {
            var result = await _service.Delete(id);
            return result.GetResponse();
        }
    }
}
