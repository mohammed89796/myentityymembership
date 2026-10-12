using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class PaymentConfigurations : IEntityTypeConfiguration<PaymentInvoice>
    {
        public void Configure(EntityTypeBuilder<PaymentInvoice> builder)
        {
            builder.HasIndex(x => x.TransactionId).IsUnique();
            builder.HasIndex(x => x.PaymentIntentId).IsUnique();




        }
    }
}
