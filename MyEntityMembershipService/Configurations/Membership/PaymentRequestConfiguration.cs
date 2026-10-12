using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class PaymentRequestConfiguration : IEntityTypeConfiguration<PaymentRequest>
    {
        public void Configure(EntityTypeBuilder<PaymentRequest> builder)
        {
            builder.HasOne(request => request.MembershipRequest)
                   .WithMany(type => type.PaymentRequests)
                   .HasForeignKey(request => request.MembershipRequestId);

            builder.HasMany(request => request.Transactions)
                   .WithOne(type => type.PaymentRequest)
                   .HasForeignKey(request => request.PaymentRequestId);

        }
    }
}
