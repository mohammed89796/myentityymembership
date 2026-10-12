namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class AddAreaDto
    {
        public int? FederationId { get; set; }
        public string Name { get; set; }
        public List<BankAccountDto>? BankAccounts { get; set; }
    }
}
