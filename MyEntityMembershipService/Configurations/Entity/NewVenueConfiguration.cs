using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class NewVenueConfiguration : IEntityTypeConfiguration<NewVenue>
    {
        public void Configure(EntityTypeBuilder<NewVenue> builder)
        {

            builder.HasOne(f => f.Facility)
                  .WithMany(o => o.NewVenues)
                  .HasForeignKey(f => f.FacilityId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vs => vs.OperatingHours)
                   .WithOne(s => s.NewVenue)
                   .HasForeignKey<VenueOperatingHours>(v => v.NewVenueId);

            builder.HasMany(v => v.VenueReviews)
                    .WithOne(r => r.Venue)
                    .HasForeignKey(r => r.VenueID)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(vs => vs.AccessedFacilitiesVenues)
                   .WithOne(s => s.Venue)
                   .HasForeignKey(vs => vs.VenueId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict); 
        }


    }
}
