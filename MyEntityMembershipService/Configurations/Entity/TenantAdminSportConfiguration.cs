using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class TenantAdminSportConfiguration : IEntityTypeConfiguration<TenantAdminSport>
    {
        public void Configure(EntityTypeBuilder<TenantAdminSport> builder)
        {
            builder.HasOne(ts => ts.TenantAdmin)
                  .WithMany(t => t.TenantAdminSports)
                  .HasForeignKey(ts => ts.TenantAdminId);
        }
    }
}
