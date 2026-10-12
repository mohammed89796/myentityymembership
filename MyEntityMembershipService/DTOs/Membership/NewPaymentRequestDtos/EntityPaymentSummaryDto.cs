namespace MyEntityMembershipService.DTOs.Membership.NewPaymentRequestDtos
{
    public class EntityPaymentSummaryDto
    {
        public List<MembersPaymentDto> MembersPaymentDto { get; set; } = new List<MembersPaymentDto>();

        public double FormFees { get; set; }

        public double TotalFormFees { get; set; }


        public double TotalFees { get; set; }

        public double BranchFee { get; set; }

        public double OperatingFees { get; set; }

        public double Vat { get; set; }

        public double? TotalAmountDue { get; set; }
    }
}
