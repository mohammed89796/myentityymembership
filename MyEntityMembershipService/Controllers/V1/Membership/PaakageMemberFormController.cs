using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.PaakageMemberFormService;

using System.Threading.Tasks;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class PaakageMemberFormController : ControllerBase
    {
        private readonly IPaakageMemberFormService paakageMemberFormService;

        public PaakageMemberFormController(IPaakageMemberFormService paakageMemberFormService)
        {
            this.paakageMemberFormService = paakageMemberFormService;
        }

        [HttpPost("AddPaakageMemberForm")]
        public async Task<IResult> AddPaakageMemberForm([FromBody] List<PaakageMemberFormDto> dtos)
        {
            var result = await paakageMemberFormService.AddPaakageMemberForms(dtos);
            return result.GetResponse();
        }

        [HttpPut("UpdatePaakageMemberForm")]
        public async Task<IResult> UpdatePaakageMemberForm([FromBody] PaakageMemberFormDto dto)
        {
            var result = await paakageMemberFormService.UpdatePaakageMemberForm(dto);
            return result.GetResponse();
        }

        [HttpDelete("DeletePaakageMemberForm/{id}")]
        public async Task<IResult> DeletePaakageMemberForm(int id)
        {
            var result = await paakageMemberFormService.DeletePaakageMemberForm(id);
            return result.GetResponse();
        }

        [HttpGet("GetAllPaakageMemberForms")]
        public async Task<IResult> GetAllPaakageMemberForms()
        {
            var result = await paakageMemberFormService.GetAllPaakageMemberForms();
            return result.GetResponse();
        }

        [HttpGet("GetPaakageMemberFormById/{id}")]
        public async Task<IResult> GetPaakageMemberFormById(int id)
        {
            var result = await paakageMemberFormService.GetPaakageMemberFormById(id);
            return result.GetResponse();
        }

        [HttpPut("SyncPaakageMemberForms/{packageId}")]
        public async Task<IResult> SyncPaakageMemberForms(int packageId, [FromBody] List<PaakageMemberFormDto> newForms)
        {
            var result = await paakageMemberFormService.SyncPaakageMemberForms(packageId, newForms);
            return result.GetResponse();
        }

    }
}
