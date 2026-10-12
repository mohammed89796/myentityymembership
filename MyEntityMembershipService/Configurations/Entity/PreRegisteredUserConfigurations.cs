using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class PreRegisteredUserConfigurations : IEntityTypeConfiguration<PreRegisteredUser>
    {
        public void Configure(EntityTypeBuilder<PreRegisteredUser> builder)
        {
            builder.HasOne(pru => pru.Tenant)
                   .WithMany(t => t.PreRegisteredUser)
                   .HasForeignKey(pru => pru.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pru => pru.FileNumber).IsUnique();
        }
    }
}
