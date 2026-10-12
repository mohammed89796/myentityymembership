using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.NewMembershipDtos
{
    public class GetAllUserMemberships
    {
        public int? Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantImage{ get; set; }
        public string UserPhoto { get; set; }
        public string MemberShipId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string NationalID { get; set; }

        public string BeltName {  get; set; }
        public MembershipStatus MembershipStatus { get; set; }
        public string PackageName { get; set; }
        public string PackageId { get; set; }
        public List<string> Branches { get; set; }
      

     

    }
}
