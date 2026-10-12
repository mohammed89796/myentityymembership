using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class NewFacilityConfiguration : IEntityTypeConfiguration<NewFacility>
    {
        public void Configure(EntityTypeBuilder<NewFacility> builder)
        {
            // Primary Key
            builder.HasKey(f => f.Id);

            builder.Property(f => f.FacilityName)
                .IsRequired();

            // NewFacility to Branch: Many-to-One
            builder.HasOne(f => f.Branch)
                .WithMany(b => b.NewFacilities)
                .HasForeignKey(f => f.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // NewFacility to FacilityLocation: One-to-One
            builder.HasOne(f => f.FacilityLocation)
                .WithOne(fl => fl.Facility)
                .HasForeignKey<FacilityLocation>(fl => fl.FacilityID)
                .OnDelete(DeleteBehavior.Cascade);

            // NewFacility to FacilityAmenities: One-to-Many
            builder.HasMany(f => f.FacilityAmenities)
                .WithOne(na => na.Facility)
                .HasForeignKey(na => na.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            // NewFacility to FacilityServices: One-to-Many
            builder.HasMany(f => f.FacilityServices)
                .WithOne(fs => fs.Facility)
                .HasForeignKey(fs => fs.FacilityID)
                .OnDelete(DeleteBehavior.Cascade);

            // NewFacility to FacilityAccessibilityOptions: One-to-Many
            builder.HasMany(f => f.FacilityAccessibilityOptions)
                .WithOne(fao => fao.Facility)
                .HasForeignKey(fao => fao.FacilityID)
                .OnDelete(DeleteBehavior.Cascade);

            // NewFacility to SportsList: One-to-Many
            builder.HasMany(f => f.SportsList)
                .WithOne(ns => ns.Facility)
                .HasForeignKey(ns => ns.FacilityID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(vs => vs.AccessedBranchesFacilities)
                .WithOne(s => s.Facility)
                .HasForeignKey(vs => vs.FacilityId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary
        }

    }
}
