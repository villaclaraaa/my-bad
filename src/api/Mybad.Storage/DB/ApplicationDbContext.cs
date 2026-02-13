using Microsoft.EntityFrameworkCore;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<WardEntity> Wards { get; set; } = default!;
    public DbSet<ParsedMatchWardInfo> ParsedMatchWardInfos { get; set; } = default!;
    public DbSet<HeroMatchupEnemyEntity> HeroMatchupEnemies { get; set; } = default!;
    public DbSet<HeroMatchupAllyEntity> HeroMatchupAllies { get; set; } = default!;
    public DbSet<CheckedMatchMatchupEntity> CheckedMatches { get; set; } = default!;
    public DbSet<HeroMatchesEntity> HeroesMatches { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WardEntity>(b =>
        {
            b.HasKey(w => new { w.MatchId, w.AccountId, w.PosX, w.PosY });

            b.HasIndex(w => w.MatchId);
            b.HasIndex(w => w.AccountId);
            b.HasIndex(w => new { w.MatchId, w.AccountId, w.PosX, w.PosY });
            b.ToTable("wards");
        });

        modelBuilder.Entity<ParsedMatchWardInfo>(b =>
        {
            b.HasKey(p => new { p.MatchId, p.AccountId });

            b.HasIndex(p => new { p.MatchId, p.AccountId });
            b.ToTable("wards_parsed_matches");
        });

        modelBuilder.Entity<HeroMatchupEntity>(hm =>
        {
            hm.HasKey(hm => new { hm.HeroId, hm.OtherHeroId, hm.PatchId });
            hm.UseTpcMappingStrategy(); // this tells to create two separate tables for matchups
            hm.ToTable((string?)null);
        });

        modelBuilder.Entity<HeroMatchupAllyEntity>().ToTable("matchup_allies");

        modelBuilder.Entity<HeroMatchupEnemyEntity>().ToTable("matchup_enemies");

        modelBuilder.Entity<CheckedMatchMatchupEntity>(cm =>
        {
            cm.ToTable("matchup_checked_matches");
            cm.HasKey(hm => new { hm.MatchId });
        });

        modelBuilder.Entity<HeroMatchesEntity>(cm =>
        {
            cm.ToTable("heroes_matches_counts");
            cm.HasKey(hm => new { hm.HeroId, hm.PatchId });
        });
    }
}