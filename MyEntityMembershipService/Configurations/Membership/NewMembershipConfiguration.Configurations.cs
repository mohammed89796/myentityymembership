using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class NewMembershipConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<NewMembershipConfiguration>
    {
        public void Configure(EntityTypeBuilder<NewMembershipConfiguration> builder)
        {

            // Define the unique constraint on PackageId
            builder.HasIndex(b => b.PackageId).IsUnique();

            // Define the foreign key for FormID
            builder.HasOne(b => b.Form)
                   .WithMany(b => b.NewMembershipConfigurations) // Assuming Form has no explicit collection of NewMembershipConfigurations
                   .HasForeignKey(b => b.FormId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}