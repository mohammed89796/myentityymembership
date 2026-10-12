namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class BranchWrapperDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public string BranchLocation { get; set; }
        public string BranchLatitude { get; set; }
        public string BranchLongitude { get; set; }
    }
}
