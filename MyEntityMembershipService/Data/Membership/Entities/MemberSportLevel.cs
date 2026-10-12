namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MemberSportLevel:BaseEntity
    {
        public string UserId { get; set; }
        public Membership Membership { get; set; }
        public int MembershipId { get; set; }
        public int LevelId { get; set; }

        public string LevelName { get; set; }
        public string LevelNameAr { get; set; }

        public int OptionId { get; set; }

        public string OptionName { get; set; }
        public string OptionNameAr { get; set; }
        public int SportId { get; set; }
        public string SportName { get; set; }
        public string SportNameAr {  get; set; }
        public string? SportImage { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryNameAr { get; set; }

    }
}
