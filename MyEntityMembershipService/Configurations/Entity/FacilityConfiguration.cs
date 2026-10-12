using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> builder)
        {
            builder.HasOne(f => f.Branch)
                   .WithMany(o => o.Facilities)
                   .HasForeignKey(f => f.BranchId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary

            builder.HasOne(vs => vs.FacilityOperatingHours)
                   .WithOne(s => s.Facility)
                   .HasForeignKey<FacilityOperatingHours>(v => v.FacilityId);

            builder.HasOne(vs => vs.FacilityType)
                   .WithMany(s => s.Facilities)
                   .HasForeignKey(vs => vs.FacilityTypeId)
                   .IsRequired(false);

            //builder.HasMany(vs => vs.AccessedBranchesFacilities)
            //       .WithOne(s => s.Facility)
            //       .HasForeignKey(vs => vs.FacilityId)
            //       .IsRequired(false)
            //       .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary
        }
    }
}
