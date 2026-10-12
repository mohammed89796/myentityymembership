using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs
{
    public class MyMembershipResult
    {
        public int Id { get; set; }
        public string TenantName { get; set; }
        public string PackageName { get; set; }
        public string? UserImage { get; set; } //FIXME: Add user Image, ask gamal for table name.
        public string UserName { get; set; }
        public int MembershipNumber { get; set; }
        public string? ParentEntity { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? Issue { get; set; }
        public string NationalID { get; set; }
        public string Branch { get; set; }
        public DateOnly Expiry { get; set; }
    }
    public class MyRequestsResult
    {
        public int Id { get; set; }
        public string TenantName { get; set; }
        public string PackageName { get; set; }
        public string UserName { get; set; }
        public string RequestType { get; set; }
        public string? ParentEntity { get; set; }
        public DateTime? DateOfSubmittion { get; set; }
        public int RequestStatusId { get; set; }
        public string RequestStatus { get; set; }
    }
}
