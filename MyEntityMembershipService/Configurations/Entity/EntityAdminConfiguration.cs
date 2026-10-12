using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entity.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class EntityAdminConfiguration : IEntityTypeConfiguration<EntityAdmin>
    {
        public void Configure(EntityTypeBuilder<EntityAdmin> builder)
        {

            builder.Property(e => e.EntityAdminStatus)
                .HasDefaultValue(EntityAdminStatus.PendingActivation);


            builder.HasOne(e => e.Tenant)
                .WithMany(s => s.EntityAdmins)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }


}
