using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class CountryCodes : BaseEntity
    {
        public string FlagURL { get; set; } = "";
        public string Code { get; set; }

    }
}
