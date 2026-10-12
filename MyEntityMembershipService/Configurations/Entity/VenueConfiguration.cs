using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class VenueConfiguration : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.HasOne(f => f.Facility)
                   .WithMany(o => o.Venues)
                   .HasForeignKey(f => f.FacilityId)
                   .OnDelete(DeleteBehavior.Cascade); // Ensure cascading delete if needed

            builder.HasOne(vs => vs.VenueOperatingHours)
                   .WithOne(s => s.Venue)
                   .HasForeignKey<VenueOperatingHours>(v => v.VenueId);

            //builder.HasMany(vs => vs.AccessedFacilitiesVenues)
            //       .WithOne(s => s.Venue)
            //       .HasForeignKey(vs => vs.VenueId)
            //       .IsRequired(false)
            //       .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary
        }


    }
}
