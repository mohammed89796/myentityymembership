using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;

namespace MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos
{
    public class MembershipRequestWithFormDataDto
    {
        public int Id { get; set; }
        public string? BatchNumber { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime? LastStatusUpdate { get; set; }
        public RequestTypeDto RequestType { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? PackageId { get; set; }
        public string PackageName {get; set; }
        public PackageSummaryDto? Package { get; set; }
        public double? FormFees { get; set; }

        public int TenantId { get; set; }
        public string? TenantName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserPhoto { get; set; }
        public string FileNumber { get; set; }
        public bool IsParent { get; set; }
        public MembershipRequestAccessTreeDto AccessTree { get; set; } = new();

        public GetParentFormUserResponseDto FormUserResponse { get; set; }
        public List<MembershipRequestWithFormDataDto>? SubMembers { get; set; } = new List<MembershipRequestWithFormDataDto>();


    }
        public class GetParentFormUserResponseDto
    {
        public int FormId { get; set; }

        public string? UserId { get; set; }
            public IEnumerable<ParentFormFieldResponseDto> FieldsResponse { get; set; }
        }
    public class ParentFormFieldResponseDto
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public int Index {  get; set; }
        public string? Response { get; set; }
        public bool? IsHidden { get; set; }
    }
}
