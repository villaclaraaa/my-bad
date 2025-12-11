using Microsoft.EntityFrameworkCore;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB;

public class ApiDbContext : DbContext
{
	public ApiDbContext(DbContextOptions<ApiDbContext> options)
		: base(options)
	{
	}

	public DbSet<TgChatIdEntity> TgChats { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TgChatIdEntity>(e =>
		{
			e.HasKey(e => e.ChatId);

			e.ToTable("tg_ids_subsribed");
		});
	}
}
