using MyEntityMembershipService.Configurations.Membership;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Membership : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? MembershipType { get; set; }
        public MembershipStatus? MembershipStatus { get; set; }
        public string? NationalId { get; set; }
        public string? Nationality { get; set; }
        public string? NationalIDPhoto { get; set; }
        public string? UserPhoto { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Governate { get; set; }
        public int? PackageId { get; set; }
        public Package? Package { get; set; }
        public string? FileNumber { get; set; }

        public string? BeltName { get; set; }
        public string? DateOfAcquisition { get; set; }

        public string? BeltCertificate { get; set; }
        public string? MilitaryStatus { get; set; }
        public string? HomeNumber { get; set; }
        public string? SchoolOrWorkEntity { get; set; }
        public string? PassportNumber { get; set; }
        public DateOnly? DateOfExpiry { get; set; }
        public DateOnly? DateOfIssuance { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        //Navigation Properties
        public string UserId { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

        // Self-referencing relationship for sub-membership
        public int? ParentMembershipId { get; set; }
        public int? SuperPackageId { get; set; }
        public bool IsMainMembership { get; set; }


        public virtual Membership ParentMembership { get; set; }
        public int? MemberTypeId { get; set; }
        public MemberType MemberType { get; set; }
        public InjuryStatus? InjuryStatus { get; set; }
        public virtual ICollection<Membership> SubMembers { get; set; } = new List<Membership>();
        public virtual ICollection<MembershipRequest> MembershipRequests { get; set; } = new List<MembershipRequest>();
        public List<FormUserResponse> FormUserResponses { get; set; }
        public List<FormUserResponse> SubMemberFormUserResponses { get; set; }  // Separate collection


        public string MemberShipId { get; set; } = default!;

        public ICollection<TournamentResult> TournamentResults { get; set; } = new HashSet<TournamentResult>();
        public ICollection<InjuryReport> InjuryReports { get; set; } = new HashSet<InjuryReport>();

        public ICollection<MembershipAccess> MembershipAccesses { get; set; } = new HashSet<MembershipAccess>();
        public ICollection<MemberSportLevel> MemberSportLevels { get; set; }= new HashSet<MemberSportLevel>();

        public ICollection<MembershipProduct> MembershipProducts { get; set; } = [];



    }
}
