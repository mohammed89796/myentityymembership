using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entity.Entities;
using MyEntityMembershipService.Data.Entity.Entities.Enums;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class EntityMemeberConfiguration : IEntityTypeConfiguration<EntityMember>
    {
        public void Configure(EntityTypeBuilder<EntityMember> builder)
        {
            builder.HasOne(e => e.Belt)
                .WithMany(s => s.EntityMembers)
                .HasForeignKey(e => e.BeltId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Tenant)
                .WithMany(s => s.EntityMembers)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);



        }
    }

}
