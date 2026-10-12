using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class TenantFacilitieConfiguration : IEntityTypeConfiguration<TenantAdminFacility>
    {
        public void Configure(EntityTypeBuilder<TenantAdminFacility> builder)
        {
            builder.HasOne(tf => tf.TenantAdmin)
                   .WithMany(t => t.TenantAdminFacilities)
                   .HasForeignKey(tf => tf.TenantAdminId);

        }
    }
}
