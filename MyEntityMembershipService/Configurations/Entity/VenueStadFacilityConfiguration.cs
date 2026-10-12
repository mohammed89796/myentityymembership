using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class VenueStadFacilityConfiguration : IEntityTypeConfiguration<VenueStadFacility>
    {
        public void Configure(EntityTypeBuilder<VenueStadFacility> builder)
        {

            builder
            .HasKey(vs => new { vs.VenueId, vs.StadFacilityId });

            builder
                .HasOne(vs => vs.Venue)
                .WithMany(v => v.VenueStadFacilities)
                .HasForeignKey(vs => vs.VenueId);

            builder
                .HasOne(vs => vs.StadFacility)
                .WithMany(s => s.VenueStadFacilities)
                .HasForeignKey(vs => vs.StadFacilityId);
        }
    }
}
