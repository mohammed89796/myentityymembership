using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class SubMemberCategoryDto
    {
        public int MemberTypeId { get; set; }
        public string? MemberTypeName { get; set; }
        public string EnglishTitle { get; set; }
        public double Price { get; set; }
        public string ArabicTitle { get; set; }
    }
}
