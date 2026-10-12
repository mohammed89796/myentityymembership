using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class NewPaymentRequestConfiguration : IEntityTypeConfiguration<NewPaymentRequest>
    {
        public void Configure(EntityTypeBuilder<NewPaymentRequest> builder)
        {
            builder.HasOne(x => x.MembershipRequest).WithMany(x => x.NewPaymentRequests).HasForeignKey(x => x.MembershipRequestId);

            builder.HasOne(x => x.Entity).WithMany(x => x.EntityPaymentPaymentRequests).HasForeignKey(x => x.EntityId);

            builder.HasOne(x => x.SuperEntity).WithMany(x => x.FedrationPaymentRequests).HasForeignKey(x => x.SuperEntityId);

            builder.Property(x => x.BranchAdminStatus)
                .HasDefaultValue(BranchAdminStatus.Pending);


        }
    }

}
