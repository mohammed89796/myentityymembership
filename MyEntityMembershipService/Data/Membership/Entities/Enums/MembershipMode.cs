namespace MyEntityMembershipService.Data.Membership.Entities.Enums
{
    public enum MembershipMode
    {
        FullMembership = 1,     // creates real member (main + family)
        AccessOnly = 2          // temporary / no family / no real member
    }
}
