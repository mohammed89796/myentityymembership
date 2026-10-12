namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos
{

    public class PaymentIntentionDto
    {
        public List<PaymentKey> Payment_Keys { get; set; }
        public int Intention_Order_Id { get; set; }
        public string Id { get; set; }
        public IntentionDetail Intention_Detail { get; set; }
        public string Client_Secret { get; set; }
        public List<PaymentMethod> Payment_Methods { get; set; }
        public string Special_Reference { get; set; }
        public bool Confirmed { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public object Card_Detail { get; set; }
        public string Object { get; set; }
    }
    public class PaymentKey
    {
        public int Integration { get; set; }
        public string Key { get; set; }
        public string Gateway_Type { get; set; }
        public string Iframe_Id { get; set; }
        public int Order_Id { get; set; }
    }

  
   
    public class IntentionDetail
    {
        public decimal Amount { get; set; }
        public List<ItemDto> Items { get; set; }
        public string Currency { get; set; }
        public BillingDataDto Billing_Data { get; set; }
    }

    public class PaymentMethod
    {
        public int Integration_Id { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Method_Type { get; set; }
        public string Currency { get; set; }
        public bool Live { get; set; }
        public bool Use_Cvc_With_Moto { get; set; }
    }

   


}
