using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.Text.Json;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            //builder.HasMany(p => p.PostTags)
            //    .WithOne(pt => pt.Post)
            builder.HasMany(p => p.PostTags)
          .WithMany(pt => pt.Posts);

            builder.HasOne(p => p.Tenant)
                .WithMany(t => t.Posts)
                .HasForeignKey(p => p.TenantId);

            builder.HasOne(p => p.Poll)
               .WithOne(p => p.Post)
               .HasForeignKey<Poll>(p => p.PostId);

            builder.HasMany(p => p.BookMarks)
                .WithOne(bm => bm.Post)
                .HasForeignKey(bm => bm.PostId);

            builder.HasMany(p => p.Likes)
               .WithOne(l => l.Post)
               .HasForeignKey(l => l.PostId);

            builder.HasMany(p => p.Shares)
               .WithOne(s => s.Post)
               .HasForeignKey(s => s.PostId);

            builder.Property(p => p.IsDraft)
                  .HasDefaultValue(false);

            builder.Property(p => p.IsPinned)
                  .HasDefaultValue(false);

            builder.Property(p => p.HasComments)
                   .HasDefaultValue(false);

            builder.Property(p => p.HasLikes)
                  .HasDefaultValue(false);

            builder.Property(p => p.HasShares)
                  .HasDefaultValue(false);

            builder.Property(p => p.PostPictureUrl)
     .HasConversion(
         v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions)null), // Serialize List<string> to JSON
         v => string.IsNullOrEmpty(v) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) // Deserialize JSON to List<string>
     ).Metadata
            .SetValueComparer(
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );



        }
    }
}
