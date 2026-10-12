namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class BranchMobileCardListDto
    {
        public int TotalNumber { get; set; }
        public int NumberOfPages { get; set; }
        public List<BranchMobileCardViewDto> Branches { get; set; }
    }
    public class BranchMobileCardViewDto
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public double? Rating { get; set; }
    }
}
