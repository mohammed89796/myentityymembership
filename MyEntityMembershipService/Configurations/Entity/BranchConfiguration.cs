using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Entity.Entities;


namespace MyEntityMembershipService.Configurations.Entity
{
       public class BranchConfiguration : IEntityTypeConfiguration<Branch>
       {
              public void Configure(EntityTypeBuilder<Branch> builder)
              {
                     builder.HasMany(e => e.AccessedBranches)
                            .WithOne(s => s.Branch)
                            .HasForeignKey(e => e.BranchId)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasMany(e => e.AccessedBranchesFacilities)
                            .WithOne(s => s.Branch)
                            .HasForeignKey(e => e.BranchId)
                            .OnDelete(DeleteBehavior.Cascade); // Ensure cascading delete if needed

                     builder.HasMany(e => e.AccessedFacilitiesVenues)
                            .WithOne(s => s.Branch)
                            .HasForeignKey(e => e.BranchId)
                            .OnDelete(DeleteBehavior.Cascade); // Ensure cascading delete if needed

                     builder.HasOne(b => b.BranchLocation)
                            .WithOne(l => l.Branch)
                            .HasForeignKey<BranchLocation>(l => l.BranchID)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(b => b.BranchOperatingHours)
                            .WithOne(l => l.Branch)
                            .HasForeignKey<BranchOperatingHours>(l => l.BranchID)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasMany(b => b.BranchImages)
                            .WithOne(i => i.Branch)
                            .HasForeignKey(i => i.BranchID)
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasMany(b => b.BranchServices)
                            .WithOne(s => s.Branch)
                            .HasForeignKey(s => s.BranchID)
                            .OnDelete(DeleteBehavior.Cascade);
              }
       }
}
