using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class TenantVenueConfiguration : IEntityTypeConfiguration<TenantAdminVenue>
    {
        public void Configure(EntityTypeBuilder<TenantAdminVenue> builder)
        {
            builder.HasOne(tv => tv.TenantAdmin)
                   .WithMany(t => t.TenantAdminVenues)
                   .HasForeignKey(tv => tv.TenantAdminId);
        }
    }
}
