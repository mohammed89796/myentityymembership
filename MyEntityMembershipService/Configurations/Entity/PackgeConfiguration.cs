using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class PackgeConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasMany(e => e.Branches)
                   .WithOne(s => s.Package)
                   .HasForeignKey(e => e.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Facilities)
                   .WithOne(s => s.Package)
                   .HasForeignKey(e => e.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Venues)
                   .WithOne(s => s.Package)
                   .HasForeignKey(e => e.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.SubMemberCategories)
                   .WithOne(s => s.Package)
                   .HasForeignKey(e => e.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.SuperPackage)                
                   .WithMany(p => p.SubPackages)                
                   .HasForeignKey(p => p.SuperPackageId)
                   .OnDelete(DeleteBehavior.Restrict);          

        }
    }
}
