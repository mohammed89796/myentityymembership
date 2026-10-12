namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class MobileBranchFacilitiesDto
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public int SportsCount { get; set; }
        public List<string> Sports { get; set; }
        public double? Rating { get; set; }
    }
   
}
