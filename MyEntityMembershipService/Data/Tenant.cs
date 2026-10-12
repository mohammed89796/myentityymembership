using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }


        public int? SuperEntityId { get; set; }
        public Tenant SuperEntity { get; set; }
        //Navigation Properties



        public int? EntityTypeId { get; set; }
        public EntityType EntityType { get; set; }
        public int? FedrationEntityTypeId { get; set; }
        public FedrationEntityType FedrationEntityType { get; set; }

        //public int? FedrationBranchId { get; set; }


        public ICollection<Tenant> Entites { get; set; } = new HashSet<Tenant>();
        public ICollection<TenantArea> TenantAreas { get; set; } = new HashSet<TenantArea>();

        public ICollection<TenantAdmin> TenantAdmins { get; set; } = new HashSet<TenantAdmin>();

        public Area Area { get; set; }
        public int? AreaId { get; set; }
        public int? FedrationBranchId { get => AreaId; set => AreaId = value; }
        public Dashboard? Dashboard { get; set; }



        public ICollection<MembershipRequest> EntityMembershipRequests { get; set; } = new HashSet<MembershipRequest>();
        public ICollection<MembershipRequest> FedrationMembershipRequests { get; set; } = new HashSet<MembershipRequest>();
        public ICollection<PaymentBatch> EntityPaymentBatches { get; set; } = new HashSet<PaymentBatch>();
        public ICollection<PaymentBatch> FedrationPaymentBatches { get; set; } = new HashSet<PaymentBatch>();
        public ICollection<NewPaymentRequest> EntityPaymentPaymentRequests { get; set; } = new HashSet<NewPaymentRequest>();
        public ICollection<NewPaymentRequest> FedrationPaymentRequests { get; set; } = new HashSet<NewPaymentRequest>();
        public virtual ICollection<Post> Posts { get; set; } = [];
        public virtual ICollection<Branch>? Branches { get; set; } = new List<Branch>();
        public ICollection<Package> Packages { get; set; } = new List<Package>();
        public ICollection<EntityMember> EntityMembers { get; set; } = new HashSet<EntityMember>();
        public ICollection<EntityAdmin> EntityAdmins { get; set; } = new HashSet<EntityAdmin>();
        public ICollection<PreRegisteredUser> PreRegisteredUser { get; set; } = new HashSet<PreRegisteredUser>();

        public ICollection<Attainment> Attainments { get; set; } = new HashSet<Attainment>();

        public object? SocialMediaLinks { get; set; }

    }
}
