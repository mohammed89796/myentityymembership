using Asp.Versioning;

using MyEntityMembershipService.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using MyEntityMembershipService.Services.Membership.RequestPaymentServ;
using Microsoft.AspNetCore.Authorization;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class PaymentRequestController : ControllerBase
    {
        private readonly IPaymentRequestService paymentRequestService;

        public PaymentRequestController(IPaymentRequestService paymentRequestService)
        {
            this.paymentRequestService = paymentRequestService;
        }

       
        [HttpPut("submit-payment-comments")]
        public async Task<IResult> SubmitPaymentComments(int tenantId, int paymentId, string comments)
        {
            var result = await paymentRequestService.SubmitPaymentComments(tenantId, paymentId, comments);
            return result.GetResponse();
        }

        [HttpGet("GetPaymentSummary/{year}/{tenantId}")]
        public async Task<IResult> GetPaymentSummary(int year, int tenantId)
        {
            var result = await paymentRequestService.GetDashboardSummaryAsync(year, tenantId);
            return result.GetResponse();

        }

    }
}

