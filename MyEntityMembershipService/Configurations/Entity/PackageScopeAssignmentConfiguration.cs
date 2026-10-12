using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class PackageScopeAssignmentConfiguration : IEntityTypeConfiguration<PackageScopeAssignment>
    {
        public void Configure(EntityTypeBuilder<PackageScopeAssignment> builder)
        {
            builder.HasKey(ps => ps.Id);

            builder.HasOne(ps => ps.Package)
                .WithMany(p => p.PackageScopeAssignments)
                .HasForeignKey(ps => ps.PackageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ps => ps.PackageScope)
                .WithMany(s => s.PackageScopeAssignments)
                .HasForeignKey(ps => ps.PackageScopeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
