namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class GetAvailableMembersRequest
    {
        public int PackageId { get; set; }
        public int RequestTypeId { get; set; }
    }

    public class AvailableMemberDto
    {
        public int MemberId { get; set; }
        public int MembershipId { get; set; }
        public string Name { get; set; }
        public int MemberTypeId { get; set; }
        public string MemberTypeName { get; set; }
        public string Status { get; set; }
        public string Avatar { get; set; }
    }
}
