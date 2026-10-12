using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class AreaConfigurations : IEntityTypeConfiguration<AreaConfiguration>
    {
        public void Configure(EntityTypeBuilder<AreaConfiguration> builder)
        {
            builder.HasIndex(builder => new { builder.AreaId, builder.OperationId }).IsUnique();

            builder.HasOne(b => b.Operation).WithMany(Operation => Operation.AreaConfigurations)
                .HasForeignKey(b => b.OperationId);

        }
    }
}
