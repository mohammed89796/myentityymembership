namespace MyEntityMembershipService.DTOs.Membership.Swimming
{
    public class MemberLevelDto
    {
        public int? LevelId { get; set; }
        public string? LevelName { get; set; }
        public int? SportId { get; set; }
        public string? SportName { get; set; }

        public string? LevelAndSport {  get; set; }
        public string? LevelAndSportAr {  get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryNameAr { get; set; }


    }
}
