using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class PackageProductConfiguration : IEntityTypeConfiguration<PackageProduct>
    {
        public void Configure(EntityTypeBuilder<PackageProduct> builder)
        {
            builder.HasKey(pp => pp.Id);

            builder.HasOne(pp => pp.Package)
                    .WithMany(p => p.PackageProducts)
                    .HasForeignKey(pp => pp.PackageId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(pp => pp.Product)
                    .WithMany(p => p.PackageProducts)
                    .HasForeignKey(pp => pp.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
