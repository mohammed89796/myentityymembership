using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class FacilityAmenitiesConfiguration : IEntityTypeConfiguration<FacilityAmenities>
    {
        public void Configure(EntityTypeBuilder<FacilityAmenities> builder)
        {
            builder
            .HasKey(vs => new { vs.FacilityId, vs.StadFacilityId });

            builder
                .HasOne(vs => vs.Facility)
                .WithMany(v => v.FacilityAmenities)
                .HasForeignKey(vs => vs.FacilityId);

            builder
                .HasOne(vs => vs.StadFacility)
                .WithMany(s => s.FacilityAmenities)
                .HasForeignKey(vs => vs.StadFacilityId);
        }
    }
}
