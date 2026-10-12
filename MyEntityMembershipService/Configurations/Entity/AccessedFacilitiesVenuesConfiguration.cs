using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entity.Entities;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class AccessedFacilitiesVenuesConfiguration : IEntityTypeConfiguration<AccessedFacilitiesVenues>
    {
        public void Configure(EntityTypeBuilder<AccessedFacilitiesVenues> builder)
        {
            builder.HasKey(afv => new { afv.PackageId, afv.BranchId, afv.FacilityId, afv.VenueId });

            builder.HasOne(afv => afv.Package)
                   .WithMany(p => p.Venues)
                   .HasForeignKey(afv => afv.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(afv => afv.Branch)
                   .WithMany(b => b.AccessedFacilitiesVenues)
                   .HasForeignKey(afv => afv.BranchId)
                   .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary

            builder.HasOne(afv => afv.Facility)
                   .WithMany(f => f.AccessedFacilitiesVenues)
                   .HasForeignKey(afv => afv.FacilityId)
                   .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary

            builder.HasOne(afv => afv.Venue)
                   .WithMany(v => v.AccessedFacilitiesVenues)
                   .HasForeignKey(afv => afv.VenueId)
                   .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary
        }
    }
}
