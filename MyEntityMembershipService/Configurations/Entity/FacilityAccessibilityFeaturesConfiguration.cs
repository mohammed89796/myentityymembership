using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class FacilityAccessibilityFeaturesConfiguration : IEntityTypeConfiguration<FacilityAccessibilityFeatures>
    {

        public void Configure(EntityTypeBuilder<FacilityAccessibilityFeatures> builder)
        {

            builder
            .HasKey(vs => new { vs.FacilityId, vs.AccessibilityFeaturesId });

            builder
                .HasOne(vs => vs.Facility)
                .WithMany(v => v.FacilityAccessibilityFeatures)
                .HasForeignKey(vs => vs.FacilityId);

            builder
                .HasOne(vs => vs.AccessibilityFeatures)
                .WithMany(s => s.FacilityAccessibilityFeatures)
                .HasForeignKey(vs => vs.AccessibilityFeaturesId);

        }
    }
}
