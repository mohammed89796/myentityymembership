namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class AreaWithAccountsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreationDate { get; set; }
        public List<BankAccountDto>? BankAccounts { get; set; }
    }
}
