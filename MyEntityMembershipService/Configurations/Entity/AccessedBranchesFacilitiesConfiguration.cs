using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entity.Entities;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class AccessedBranchesFacilitiesConfiguration : IEntityTypeConfiguration<AccessedBranchesFacilities>
    {
        public void Configure(EntityTypeBuilder<AccessedBranchesFacilities> builder)
        {
            builder.HasKey(abf => new { abf.PackageId, abf.BranchId, abf.FacilityId });

            builder.HasOne(abf => abf.Package)
                    .WithMany(p => p.Facilities) // Ensure Package has a collection of AccessedBranchesFacilities
                    .HasForeignKey(abf => abf.PackageId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(abf => abf.Branch)
                   .WithMany(b => b.AccessedBranchesFacilities)
                   .HasForeignKey(abf => abf.BranchId)
                   .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary

            builder.HasOne(abf => abf.Facility)
                   .WithMany(f => f.AccessedBranchesFacilities)
                   .HasForeignKey(abf => abf.FacilityId)
                   .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary
        }
    }
}
