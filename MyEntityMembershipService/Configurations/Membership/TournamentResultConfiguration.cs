using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class TournamentResultConfiguration : IEntityTypeConfiguration<TournamentResult>
    {
        public void Configure(EntityTypeBuilder<TournamentResult> builder)
        {
            // Relationship to HistoricalTournament 
            builder.HasOne(r => r.HistoricalTournament)
                   .WithMany(t => t.Results)
                   .HasForeignKey(r => r.HistoricalTournamentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship to SwimmingMembership
            builder.HasOne(r => r.Membership)
                   .WithMany(s=>s.TournamentResults)
                   .HasForeignKey(r => r.MembershipId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
