namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class FreezAdditionalPayment
    {
        public bool HasAdditionalPayment { get; set; }
        public string Message { get; set; }
        public int SelectedFreezePeriod { get; set; }
        public int IncludedFreezeDays { get; set; }
        public int AdditionalDays { get; set; }
        public double CostForAdditionalDay { get; set; }
        public double TotalCost {  get; set; }
    }
}
