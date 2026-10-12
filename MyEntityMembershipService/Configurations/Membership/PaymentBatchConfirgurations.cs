using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using System.Text.Json;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class PaymentBatchConfirgurations : IEntityTypeConfiguration<PaymentBatch>
    {
        public void Configure(EntityTypeBuilder<PaymentBatch> builder)
        {

            builder.HasOne(x => x.Batch).WithMany(x => x.PaymentBatches).HasForeignKey(x => x.BatchId);

            builder.HasOne(x => x.Entity).WithMany(x => x.EntityPaymentBatches).HasForeignKey(x => x.EntityId);

            builder.HasOne(x => x.SuperEntity).WithMany(x => x.FedrationPaymentBatches).HasForeignKey(x => x.SuperEntityId);

            builder.HasMany(x => x.NewPaymentRequests).WithOne(x => x.PaymentBatch).HasForeignKey(x => x.PaymentBatchId);

            builder.Property(x => x.BranchAdminStatus)
                .HasDefaultValue(BranchAdminStatus.Pending);

            builder.Property(pb => pb.InvoiceUrl)
    .HasConversion(
        v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions)null), // Serialize List<string> to JSON
        v => string.IsNullOrEmpty(v) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) // Deserialize JSON to List<string>
    ).Metadata
           .SetValueComparer(
               new ValueComparer<List<string>>(
                   (c1, c2) => c1.SequenceEqual(c2),
                   c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                   c => c.ToList()
               )
           );

        }
    }

}
