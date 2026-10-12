using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.SwimmingMembershipRequestDtos
{
    public class ChildRequestDto
    {
        public int Id { get; set; }
        public string? EmailAddress { get; set; }
        public string? ChildFullName { get; set; }
        public string? BatchNumber { get; set; }    
        public string? PackageName { get; set; }
        public string? NationalId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Level { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public int? PayemntId { get; set; }
        public int? SuperMembershipId { get; set; } 
        public RequestStatus? RequestStatus { get; set; }
        public bool IsParent { get; set; }

        public int? MemberTypeId { get; set; }

        public bool IsIndividual { get; set; }

    }
}
