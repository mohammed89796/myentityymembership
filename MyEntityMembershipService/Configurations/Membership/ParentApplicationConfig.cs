using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class ParentApplicationConfig : IEntityTypeConfiguration<ParentApplicationConfiguration>
    {
        public void Configure(EntityTypeBuilder<ParentApplicationConfiguration> builder)
        {
            builder.HasKey(pa => pa.Id);

            builder.HasOne(x => x.Package)
                   .WithOne(x => x.ParentApplicationConfiguration)
                   .HasForeignKey<ParentApplicationConfiguration>(x => x.PackageId)
                   .OnDelete(DeleteBehavior.NoAction);



        }
    }
}
