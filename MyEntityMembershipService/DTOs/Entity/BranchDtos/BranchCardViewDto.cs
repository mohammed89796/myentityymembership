namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class BranchListDetailsDto
    {
        public int TotalNumber { get; set; }
        public int NumberOfPages { get; set; }
        public List<BranchCardViewDto> Branches { get; set; }
    }
    public class BranchCardViewDto
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
    }
}
