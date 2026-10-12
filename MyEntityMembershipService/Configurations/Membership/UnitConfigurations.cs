using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class UnitConfigurations : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasOne(unit => unit.UnitType)
                   .WithMany(type => type.Units)
                   .HasForeignKey(unit => unit.UnitTypeId);
        }
    }
}
