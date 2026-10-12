using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class TenantAdminConfiguration : IEntityTypeConfiguration<TenantAdmin>
    {
        public void Configure(EntityTypeBuilder<TenantAdmin> builder)
        {
            builder.HasMany(t => t.TenantAdminBranches)
            .WithOne(tb => tb.TenantAdmin)
            .HasForeignKey(tb => tb.TenantAdminId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.TenantAdminFacilities)
            .WithOne(tb => tb.TenantAdmin)
            .HasForeignKey(tb => tb.TenantAdminId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.TenantAdminSports)
            .WithOne(tb => tb.TenantAdmin)
            .HasForeignKey(tb => tb.TenantAdminId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.TenantAdminVenues)
            .WithOne(tb => tb.TenantAdmin)
            .HasForeignKey(tb => tb.TenantAdminId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
