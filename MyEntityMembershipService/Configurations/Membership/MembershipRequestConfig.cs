using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class MembershipRequestConfig : IEntityTypeConfiguration<MembershipRequest>
    {
        public void Configure(EntityTypeBuilder<MembershipRequest> builder)
        {
            builder.HasOne(request => request.RequestType)
                   .WithMany(type => type.MembershipRequests)
                   .HasForeignKey(request => request.RequestTypeId);

            builder.HasOne(request => request.Membership)
                   .WithMany(membership => membership.MembershipRequests)
                   .HasForeignKey(request => request.MembershipId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(request => request.CommentsAndJustification)
                   .WithOne(comment => comment.MembershipRequest)
                   .HasForeignKey(comment => comment.MembershipRequestId);

            builder.HasOne(request => request.MemberType)
                    .WithMany(type => type.MembershipRequests)
                    .HasForeignKey(request => request.MemberTypeId);

            builder.HasOne(request => request.Batch)
                .WithMany(type => type.MembershipRequests)
                .HasForeignKey(request => request.BatchId);

            builder.HasOne(request => request.Tenant)
                .WithMany(t => t.EntityMembershipRequests)
                .HasForeignKey(request => request.TenantId);

            builder.HasOne(request => request.SuperTenant)
               .WithMany(t => t.FedrationMembershipRequests)
               .HasForeignKey(request => request.SuperTenantId);

            builder.Property(pb => pb.BranchAdminStatus)
                .HasDefaultValue(BranchAdminStatus.Pending);

            builder.HasMany(request => request.MembershipRequestAccesses)
                 .WithOne(comment => comment.MembershipRequest)
                 .HasForeignKey(comment => comment.MembershipRequestId);

            builder.HasOne(request => request.Package)
             .WithMany(package => package.MembershipRequests)
             .HasForeignKey(request => request.PackageId)
             .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
