using AutoMapper;
using Microsoft.Extensions.Options;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.PaymentInvoiceRepository;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.EmailServ;
using System.Text.Json;
using PaymentRequestDto = MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos.PaymentRequestDto;

namespace MyEntityMembershipService.Services.Membership.PaymentInvoiceServ
{

    public class PaymentInvoiceService : IPaymentInvoiceService
    {
        private readonly PaymobSettings _options;
        private readonly IPaymentInvoiceRepo _paymentInvoiceRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public PaymentInvoiceService(IPaymentInvoiceRepo paymentInvoiceRepo,
                                     IUnitOfWork unitOfWork,
                                     IOptions<PaymobSettings> options,
                                     IMapper mapper,
                                     IEmailService emailService)
        {
            _options = options.Value;
            _paymentInvoiceRepo = paymentInvoiceRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<Response<string>> CheckOut(PaymentWebhookDto webhookDto)
        {
            try
            {

                if (webhookDto.Success)
                {


                    var payment = await _paymentInvoiceRepo.GetPaymentInvoiceDetailsWithIntentionIdAsync(webhookDto.Order);

                    await UpdatePaymentInvoice(payment, webhookDto, RequestStatus.PaymentComplete);

                    _emailService.SendInvoiceConfirmationEmail(payment);
                   
                    
                    return new ResponseHandler().Success("Payment Success");

                }
                else if(!webhookDto.Success && !webhookDto.Pending)
                {
                    var payment = await _paymentInvoiceRepo.GetPaymentInvoiceDetailsWithIntentionIdAsync(webhookDto.Order);

                    await UpdatePaymentInvoice(payment, webhookDto, RequestStatus.Rejected);


                    _emailService.SendInvoiceConfirmationEmail(payment);



                }
                return new ResponseHandler().Success("Payment Rejected");


            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<string>(ex.Message);
            }
        }
        private async Task UpdatePaymentInvoice(PaymentInvoice payment, PaymentWebhookDto webhookDto, RequestStatus status )
        {
            if (payment != null)
            {
                payment.UpdatedAt = webhookDto.Updated_At;
                payment.Currency = webhookDto.Currency ?? payment.Currency;
                payment.PaymentDate = DateTime.Now;
                payment.Status = status;
                payment.TransactionId = webhookDto.Id;
                _paymentInvoiceRepo.Update(payment);
                await _unitOfWork.Save();

            }
        }

        //       !!!     Amount of The Items in the List Of Items Is Converted Into Cents By Multiply it by 100 in the ItemDto !!!!
        public async Task<Response<CreatePaymentIntentionResult>> CreateorUpdatePaymentIntentId(PaymentRequestDto paymentRequestDto)
        {
            try
            {
                var response = await SendPaymobRequest(paymentRequestDto);

                string responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<PaymentIntentionDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                if (response.IsSuccessStatusCode)
                {
                    var payment = CreatePaymentInvoice(paymentRequestDto, result.Intention_Order_Id);
                    payment.Items = CreatePaymentItems(paymentRequestDto.Items);
                    
                    await _paymentInvoiceRepo.Add(payment);
                    await _unitOfWork.Save();


                    var paymentResult = CreateCheckOutUrl(result);

                    return new ResponseHandler().Success(paymentResult);
                }
                return new ResponseHandler().BadRequest<CreatePaymentIntentionResult>(response.Content.ToString());

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<CreatePaymentIntentionResult>(ex.Message);
            }

        }
        private async Task<HttpResponseMessage> SendPaymobRequest(PaymentRequestDto paymentRequestDto)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/v1/intention/");
            request.Headers.Add("Authorization", _options.SecretKey /* Secret Key */);


            var jsonBody = new
            {
                amount = paymentRequestDto.Amount, //!!!Amount of The Items in the List Of Items Is Converted Into Cents By Multiply it by 100 in the ItemDto !!!
                currency = paymentRequestDto.Currency,
                payment_methods = paymentRequestDto.PaymentMethods, // you can use name or id you get it from developers/payment integration,
                items = paymentRequestDto.Items, //!!!Amount of The Items in the List Of Items Is Converted Into Cents By Multiply it by 100 in the ItemDto !!!
                customer = paymentRequestDto.Customer,
                billing_data = new
                {
                    first_name = paymentRequestDto.Customer?.FirstName,
                    last_name = paymentRequestDto.Customer?.LastName,
                    phone_number = paymentRequestDto.Customer?.Phone,
                    email = paymentRequestDto.Customer?.Email,
                },

            };

            StringContent content = new StringContent(JsonSerializer.Serialize(jsonBody), null, "application/json");
            request.Content = content;


            return await client.SendAsync(request);
        }

        private CreatePaymentIntentionResult CreateCheckOutUrl(PaymentIntentionDto result)
        {
            var url = $"{_options.CheckoutUrl}{_options.IFrameId}?payment_token={result.Payment_Keys.FirstOrDefault().Key}";
            return new CreatePaymentIntentionResult { CheckOutLink = url };

        }

        private List<PaymentItem> CreatePaymentItems(List<ItemDto> itemDto)
        {
            var items = new List<PaymentItem>();
            foreach (var itemdto in itemDto)
            {

                var item = new PaymentItem()
                {
                    //If the Payment Success We Convert Back the amount Into Pounds From Being Cents
                    amount = itemdto.amount / 100,
                    description = itemdto.description,
                    name = itemdto.name,
                    ItemId = itemdto.ItemId,
                    PaymentTypeId = itemdto.PaymentTypeId,

                };
                items.Add(item);

            }
            return items;

        }
        private PaymentInvoice CreatePaymentInvoice(PaymentRequestDto paymentRequestDto, int intentionId)
        {
            var payment = new PaymentInvoice()
            {

                //If the Payment Success We Convert Back the amount Into Pounds From Being Cents
                Amount = paymentRequestDto.Amount / 100,
                Currency = paymentRequestDto.Currency,
                Email = paymentRequestDto.Customer.Email,
                FirstName = paymentRequestDto.Customer.FirstName,
                LastName = paymentRequestDto.Customer.LastName,
                Phone = paymentRequestDto.Customer.Phone,
                Status = RequestStatus.Pending,
                PaymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                PaymentIntentId = intentionId,
                PaymentMethod = paymentRequestDto.PaymentMethods.FirstOrDefault().ToString(),
                UserId = paymentRequestDto.UserId,
                
            };
            return payment;
        }



        public async Task<Response<List<PaymentInvoiceDto>>> GetAllInvoices()
        {
            try
            {
                var invoices = await _paymentInvoiceRepo.GetAllAsync();

                var mappedInvoices = _mapper.Map<List<PaymentInvoiceDto>>(invoices);

                return new ResponseHandler().Success(mappedInvoices);

            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<List<PaymentInvoiceDto>>(ex.Message);

            }
        }

        public async Task<Response<List<PaymentInvoiceDto>>> GetAllUserInvoice(string userId)
        {
            try
            {
                var invoices = await _paymentInvoiceRepo.GetUserPayments(userId);

                var mappedInvoices = _mapper.Map<List<PaymentInvoiceDto>>(invoices);

                return new ResponseHandler().Success(mappedInvoices);

            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<List<PaymentInvoiceDto>>(ex.Message);

            }
        }

       

        public async Task<Response<PaymentInvoiceDetailsDto>> GetPaymentInvoiceDetails(int? id, int? transactionId)
        {
            try
            {
                PaymentInvoiceDetailsDto invoiceDetailsDto;
                if (id.HasValue)
                {
                    invoiceDetailsDto = await GetPaymentInvoiceDetailsById(id.Value);
                }
                else if (transactionId.HasValue)
                {
                    invoiceDetailsDto = await GetPaymentInvoiceDetailsByTransId(transactionId.Value);

                }
                else
                {
                    return new ResponseHandler().BadRequest<PaymentInvoiceDetailsDto>("You have to provide an either invoice Id or a transaction Id");

                }
                return new ResponseHandler().Success(invoiceDetailsDto);
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<PaymentInvoiceDetailsDto>(ex.Message);

            }




        }

        private async Task<PaymentInvoiceDetailsDto> GetPaymentInvoiceDetailsById(int id)
        {
            var invoiceDetails = await _paymentInvoiceRepo.GetPaymentInvoiceDetailsAsync(id);
            var mappedInvoiceDetails = _mapper.Map<PaymentInvoiceDetailsDto>(invoiceDetails);

            return mappedInvoiceDetails;
        }
        private async Task<PaymentInvoiceDetailsDto> GetPaymentInvoiceDetailsByTransId(int transactionId)
        {
            var invoiceDetails = await _paymentInvoiceRepo.GetPaymentInvoiceDetailsWithOrderIdAsync(transactionId);
            var mappedInvoiceDetails = _mapper.Map<PaymentInvoiceDetailsDto>(invoiceDetails);

            return mappedInvoiceDetails;
        }

    }
}
