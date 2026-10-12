using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.DTOs.Membership.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos;
using MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos.FedrationEntitiesPaymentRequests;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.PaymentService;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]

    public class PaymentRequestsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;


        public PaymentRequestsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;

        }

        [HttpPost("paymentrequest")]
        public async Task<IResult> CreatePaymentRequest([FromForm] AddPaymentRequestDto dto)

          => (await _paymentService.CreatePaymentRequestAsync(dto)).GetResponse();

        [HttpPost("SkipAndCreatePindingPaymentRequest")]
        public async Task<IResult> SkipAndCreatePindingPaymentRequest([FromBody] PindingRequestsDto dto)

         => (await _paymentService.CreatePindingPaymentRequest(dto)).GetResponse();

        [HttpGet("paymentrequest")]
        public async Task<IResult> GetAllFedrationPaymentRequests([FromQuery] GetFedrationPaymentRequestDto dto)

          => (await _paymentService.GetAllFedrationPaymentRequestsAsync(dto)).GetResponse();


        [HttpGet("paymentrequest/{paymentRequestId}")]
        public async Task<IResult> GetAllFedrationPaymentRequests(int paymentRequestId)

          => (await _paymentService.GetFedrationPaymentRequestByIdAsync(paymentRequestId)).GetResponse();

        [HttpGet("batchfees/{batchId}")]
        public async Task<IResult> GetBatchFeesByIdAsync(int batchId)

          => (await _paymentService.GetBatchFeesByIdAsync(batchId)).GetResponse();


        [HttpPut("paymentConfirmation")]
        public async Task<IResult> ConfirmPaymentRequestAsync([FromForm] ConfirmPaymentDto dto)

          => (await _paymentService.ConfirmPaymentRequestAsync(dto)).GetResponse();


        [HttpGet("entitypaymentrequest")]
        public async Task<IResult> GetAllEntityPaymentRequestsAsync([FromQuery] GetPaginationDto dto)

          => (await _paymentService.GetAllEntityPaymentRequestsAsync(dto)).GetResponse();

        [HttpGet("FedrationEntitiesPaymentRequests")]
        public async Task<IResult> GetAllFedrationEntitiesPaymentRequests([FromQuery] GetFedrationEntityPaymentRequestDto dto)

      => (await _paymentService.GetAllFedrationEntitiesPaymentRequestsAsync(dto)).GetResponse();
        [HttpGet("GetEntitiyPaymentRequest")]
        public async Task<IResult> GetEntitiyPaymentRequest([FromQuery] GetEntitiyPaymentRequestDto dto)

            => (await _paymentService.GetAllEntityPaymentRequest(dto)).GetResponse();

        [HttpGet("GetPaymentRequestStatusSummary/{entityId}")]
        public async Task<IResult> GetPaymentRequestStatusSummary(int entityId)

           => (await _paymentService.GetPaymentRequestStatusSummaryAsync(entityId)).GetResponse();

        [HttpGet("GetFederationPaymentSummary")]
        public async Task<IResult> GetFederationPaymentSummary()

           => (await _paymentService.GetFederationPaymentSummaryAsync()).GetResponse();


        [HttpPost("GenereatePaymentRequests")]
        public async Task<IResult> GenereatePaymentRequests([FromBody] List<int> requestIds)
    => (await _paymentService.GenereatePaymentRequests(requestIds)).GetResponse();


        [HttpPut("entitypaymentrequest")]
        public async Task<IResult> EntityPayNowAsync([FromForm] EntityPayNowDto dto)

         => (await _paymentService.EntityPayNowAsync(dto)).GetResponse();

        [HttpPost("entitypaymentrequestssummary")]
        public async Task<IResult> GetEntityPaymentRequestSummary([FromBody] List<int> requestIds)
    => (await _paymentService.EntityPaymentSummary(requestIds)).GetResponse();


        [HttpGet("entitybankaccounts")]
        public async Task<IResult> GetEntityBankAccountsAsync()

          => (await _paymentService.GetEntityBankAcounts()).GetResponse();

        [HttpPost("fedrationpaymentrequestssummary")]
        public async Task<IResult> GetFedrationPaymentSummariesByRequestId([FromBody] List<int> requestIds)
    => (await _paymentService.GetFedrationPaymentSummariesByRequestIdsAsync(requestIds)).GetResponse();

        [HttpPost("paymentrejection")]
        public async Task<IResult> RejectPaymentAsync([FromForm] RejectPaymentDto dto)

          => (await _paymentService.RejectPaymentAsync(dto)).GetResponse();
    }
}
