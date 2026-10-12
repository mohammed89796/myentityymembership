using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class FacilitySportsAccommodatedConfiguration : IEntityTypeConfiguration<FacilitySportsAccommodated>
    {
        public void Configure(EntityTypeBuilder<FacilitySportsAccommodated> builder)
        {
            builder
            .HasKey(vs => new { vs.FacilityId, vs.SportsAccommodatedId });

            builder
                .HasOne(vs => vs.Facility)
                .WithMany(v => v.FacilitySportsAccommodated)
                .HasForeignKey(vs => vs.FacilityId);

            builder
                .HasOne(vs => vs.SportsAccommodated)
                .WithMany(s => s.FacilitySportsAccommodated)
                .HasForeignKey(vs => vs.SportsAccommodatedId);
        }
    }
}
