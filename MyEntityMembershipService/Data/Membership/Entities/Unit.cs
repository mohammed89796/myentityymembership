namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Unit :BaseEntity
    {
        public string Name { get; set; }
        public int UnitTypeId { get; set; }
        public UnitType UnitType { get; set; } 
        public List<FieldResponse> FieldResponses { get; set; }
    }
}
