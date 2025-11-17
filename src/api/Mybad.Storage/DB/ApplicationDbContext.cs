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

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<WardEntity>(b =>
		{
			b.HasKey(w => new { w.MatchId, w.AccountId, w.PosX, w.PosY });

			b.HasIndex(w => w.MatchId);
			b.HasIndex(w => w.AccountId);
			b.HasIndex(w => new { w.MatchId, w.AccountId, w.PosX, w.PosY });
		});

		modelBuilder.Entity<ParsedMatchWardInfo>(b =>
		{
			b.HasKey(p => new { p.MatchId, p.AccountId });

			b.HasIndex(p => new { p.MatchId, p.AccountId });
		});
	}
}
