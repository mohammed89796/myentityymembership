using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {

            builder.HasOne(e => e.Sport)
                             .WithMany(s => s.Teams)
                             .HasForeignKey(e => e.SportID)
                             .OnDelete(DeleteBehavior.Cascade);

           

        }
    }
}

