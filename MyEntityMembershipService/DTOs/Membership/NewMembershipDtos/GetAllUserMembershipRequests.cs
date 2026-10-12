using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;

namespace MyEntityMembershipService.DTOs.Membership.NewMembershipDtos
{
    public class GetAllUserMembershipRequests
    {
        public int? Id { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantImage { get; set; }
        public string UserPhoto { get; set; }
        public string FullName { get; set; }
        public string BeltName { get; set; }
        public RequestStatus Status { get; set; }
        public RequestTypeDto RequestType { get; set; }
        public DateTime? CreationDate { get; set; }

    }
}
