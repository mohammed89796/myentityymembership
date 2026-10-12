using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class SubMemberCategory:BaseEntity
    {
        public int MemberTypeId { get; set; }
        public MemberType MemberType { get; set; }  
        public string EnglishTitle { get; set; }
        public double Price { get; set; }
        public string ArabicTitle { get; set; }
        public Package Package { get; set; }
        public int PackageId { get; set; }
    }
}
