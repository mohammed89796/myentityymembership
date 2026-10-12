using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs
{
    public class GenericSearchDto
    {
        public string? Keyword { get; set; }
        public string[] SearchEntities { get; set; }
    }

    public class SearchResultDto
    {
        public string EntityType { get; set; }
        public object Data { get; set; }
    }

}
