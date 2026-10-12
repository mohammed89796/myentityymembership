namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class GetFreezCardDto
    {
        public int TotalFreezDaysUsed { get; set; }
        public int RemainingFreezDays {  get; set; }
        public List<PreviousFreezRequest> PreviousFreezes { get; set; }
        public bool CanExceedLimit { get; set; }
        public int FreezLimit { get; set; }
        public double PricePerAdditionalDays { get; set; }
    }
}
