using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.PaymentInvoiceServ;
using Newtonsoft.Json;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]

    public class PaymentInvoiceController : ControllerBase
    {
        private readonly IPaymentInvoiceService _service;

        public PaymentInvoiceController(IPaymentInvoiceService service)
        {
            _service = service;
        }

        [HttpGet("GetPaymentInvoiceDetails")]
        public async Task<IResult> GetPaymentInvoiceDetails(int? id, int? transactionId)
        {
            var result = await _service.GetPaymentInvoiceDetails(id, transactionId);
            return result.GetResponse();
        }
        [HttpGet("GetAllUserInvoice")]
        public async Task<IResult> GetAllUserInvoice(string userId)
        {
            var result = await _service.GetAllUserInvoice(userId);
            return result.GetResponse();
        }
      
        [HttpGet("GetAllInvoices")]
        public async Task<IResult> GetAllInvoices()
        {
            var result = await _service.GetAllInvoices();
            return result.GetResponse();
        }
        [HttpPost("PaymentWebHook")]
        public async Task<IResult> PaymentWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
           
            var paymentwebhook= JsonConvert.DeserializeObject<PaymentWebhookDto>(json);
            
            var result = await _service.CheckOut(paymentwebhook!);
            return result.GetResponse();
        }

        [HttpPost("CreateorUpdatePaymentIntentId")]
        public async Task<IResult> CreateorUpdatePaymentIntentId(PaymentRequestDto paymentRequestDto)
        {
            var result = await _service.CreateorUpdatePaymentIntentId(paymentRequestDto);
            return result.GetResponse();

        }
    }
}
