using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class MembershipRequest : BaseEntity
    {
        #region Request Metadata
        public RequestStatus Status { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
        public RequestPaymentStatus RequestPaymentStatus { get; set; }
        public RequestType RequestType { get; set; }
        public int? RequestTypeId { get; set; }
        public SubmationPlatform? SubmationPlatform { get; set; }
        public OperationType? OperationType { get; set; }
        public bool? IsTermsAgreed { get; set; }
        public bool IsParent { get; set; } 
        public int? SuperMembershipRequestId { get; set; }
        public MembershipRequest? SuperMembershipRequest { get; set; }
        public List<MembershipRequest>? SubMembers { get; set; }
        #endregion

        #region Personal Information
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? NationalId { get; set; }
        public string? PassportNumber { get; set; }
        public string? MilitaryStatus { get; set; }
        #endregion

        #region Contact Information
        public string? Email { get; set; }
        public string? SecondEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SecondPhoneNumber { get; set; }
        public string? HomeNumber { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Governate { get; set; }
        #endregion

        #region Documents & Photos
        public string? NationalIdPhoto { get; set; }
        public string? UserPhoto { get; set; }
        public string? BeltCertificate { get; set; }
        public string? BeltName { get; set; }
        public string? TransferRequestDocument { get; set; }
        public string? LoanRequestDocument { get; set; }
        #endregion

        #region Membership & Tenant Relations
        public int? MembershipId { get; set; }
        public Membership Membership { get; set; }
        public MembershipType MembershipType { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int? SuperTenantId { get; set; }
        public Tenant? SuperTenant { get; set; }
        public int? FromTenantId { get; set; }
        public Tenant? FromTenant { get; set; }
        public int? PackageId { get; set; }
        public Package? Package { get; set; }
        public int? MemberTypeId { get; set; }
        public MemberType MemberType { get; set; }
        #endregion

        #region Request Lifecycle
        public DateTime? ExpiryDate { get; set; }
        public DateOnly? DateOfIssuance { get; set; }
        public DateOnly? DateOfExpiry { get; set; }
        public string? DateOfAcquisition { get; set; }
        public string? SeparationReason { get; set; }
        public RejectionReason? RejectionReason { get; set; }
        public BranchAdminStatus BranchAdminStatus { get; set; }
        #endregion

        #region Request Tracking & Workflow
        public int? BuildUpId { get; set; }
        public int? BatchId { get; set; }
        public virtual Batch Batch { get; set; }
        public string? FileNumber { get; set; }
        public string? SchoolOrWorkEntity { get; set; }
        public List<MembershipRequestComment>? CommentsAndJustification { get; set; }
        public virtual ICollection<RequestStatusHistory>? RequestStatusHistories { get; set; } = new List<RequestStatusHistory>();
        public List<MembershipRequestBackgroundJobs> BackgroundJobs { get; set; }

        public ICollection<MembershipRequestAccess> MembershipRequestAccesses { get; set; } = new List<MembershipRequestAccess>();
        public ICollection<MembershipRequestProduct> RequestProducts { get; set; } = new HashSet<MembershipRequestProduct>();

        #endregion

        #region Payments
        public virtual ICollection<PaymentRequest>? PaymentRequests { get; set; } = new List<PaymentRequest>();
        public ICollection<NewPaymentRequest> NewPaymentRequests { get; set; } = new HashSet<NewPaymentRequest>();
        #endregion

        #region User / Form Responses
        public string? UserId { get; set; }
        public FormUserResponse FormUserResponse { get; set; }
        #endregion
    }
}
