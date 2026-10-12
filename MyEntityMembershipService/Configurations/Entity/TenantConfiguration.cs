using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using System.Text.Json;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasMany(e => e.Branches)
                               .WithOne(s => s.Tenant)
                               .HasForeignKey(e => e.TenantId)
                               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Packages)
                               .WithOne(s => s.Tenant)
                               .HasForeignKey(e => e.TenantId)
                               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.EntityType)
                             .WithMany(s => s.Tenants)
                             .HasForeignKey(e => e.EntityTypeId)
                             .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.SuperEntity)
                             .WithMany(s => s.Entites)
                             .HasForeignKey(e => e.SuperEntityId)
                             .OnDelete(DeleteBehavior.SetNull);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            builder.Property(t => t.SocialMediaLinks).HasColumnType("json").HasConversion(
        v => JsonSerializer.Serialize(v, options),
        v => string.IsNullOrWhiteSpace(v)
             ? new List<SocialMediaDto>()
             : JsonSerializer.Deserialize<List<SocialMediaDto>>(v, options) ?? new List<SocialMediaDto>()
    ); ;
        }

    }
}

