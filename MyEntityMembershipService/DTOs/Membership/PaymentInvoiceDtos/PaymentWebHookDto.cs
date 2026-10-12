namespace MyEntityMembershipService.DTOs.Membership.PaymentInvoiceDtos
{
    public class PaymentWebhookDto
    {
        public int Id { get; set; }
        public bool Pending { get; set; }
        public int Amount_Cents { get; set; }
        public bool Success { get; set; }
        public bool Is_Auth { get; set; }
        public bool Is_Capture { get; set; }
        public bool Is_Standalone_Payment { get; set; }
        public bool Is_Voided { get; set; }
        public bool Is_Refunded { get; set; }
        public bool Is_3D_Secure { get; set; }
        public int Integration_Id { get; set; }
        public int Profile_Id { get; set; }
        public bool Has_Parent_Transaction { get; set; }
        public int Order { get; set; }
        public DateTime Created_At { get; set; }
        public string Currency { get; set; }
        public int Merchant_Commission { get; set; }
        public string? Discount_Details { get; set; }
        public bool Is_Void { get; set; }
        public bool Is_Refund { get; set; }
        public bool Error_Occurred { get; set; }
        public int Refunded_Amount_Cents { get; set; }
        public int Captured_Amount { get; set; }
        public DateTime Updated_At { get; set; }
        public bool Is_Settled { get; set; }
        public bool Bill_Balanced { get; set; }
        public bool Is_Bill { get; set; }
        public int Owner { get; set; }
        public Data? Data { get; set; }
        public Source_data? Source_Data { get; set; }
        public string? Acq_Response_Code { get; set; }
        public string? Txn_Response_Code { get; set; }
        public string? Hmac { get; set; }
    }

    public class Data
    {
        public string Message { get; set; }
    }

    public class Source_data
    {
        public string Type { get; set; }
        public int Pan { get; set; }
        public string Sub_Type { get; set; }
    }
}
