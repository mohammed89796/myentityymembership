using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entity.Entities;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class AccessedBranchesConfiguration : IEntityTypeConfiguration<AccessedBranches>
    {
        public void Configure(EntityTypeBuilder<AccessedBranches> builder)
        {
            builder.HasKey(ab => new { ab.PackageId, ab.BranchId });

            builder.HasOne(ab => ab.Package)
                   .WithMany(p => p.Branches)
                   .HasForeignKey(ab => ab.PackageId)
                   .OnDelete(DeleteBehavior.Cascade); // Ensure cascading delete if needed

            builder.HasOne(ab => ab.Branch)
                   .WithMany(b => b.AccessedBranches)
                   .HasForeignKey(ab => ab.BranchId)
                   .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary
        }
    }
}
