using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class TenantBranchConfiguration : IEntityTypeConfiguration<TenantAdminBranch>
    {
        public void Configure(EntityTypeBuilder<TenantAdminBranch> builder)
        {
            builder.HasOne(tb => tb.TenantAdmin)
                   .WithMany(t => t.TenantAdminBranches)
                   .HasForeignKey(tb => tb.TenantAdminId);
        }
    }
}
