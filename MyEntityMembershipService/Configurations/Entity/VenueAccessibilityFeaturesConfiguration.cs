using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class VenueAccessibilityFeaturesConfiguration : IEntityTypeConfiguration<VenueAccessibilityFeatures>
    {
        public void Configure(EntityTypeBuilder<VenueAccessibilityFeatures> builder)
        {

            builder
            .HasKey(vs => new { vs.VenueId, vs.AccessibilityFeaturesId });

            builder
                .HasOne(vs => vs.Venue)
                .WithMany(v => v.VenueAccessibilityFeatures)
                .HasForeignKey(vs => vs.VenueId);

            builder
                .HasOne(vs => vs.AccessibilityFeatures)
                .WithMany(s => s.VenueAccessibilityFeatures)
                .HasForeignKey(vs => vs.AccessibilityFeaturesId);

        }
    }
}
