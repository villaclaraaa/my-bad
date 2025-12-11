using Microsoft.EntityFrameworkCore;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Services;

/// <summary>
/// Provides methods for managing Telegram bot chat subscriptions in the application database.
/// </summary>
public class TgBotSubscriberService
{
	private readonly ApiDbContext _ctx;

	public TgBotSubscriberService(ApiDbContext context)
	{
		_ctx = context;
	}

	/// <summary>
	/// Adds a chat subscription for the specified chat identifier if it does not already exist.
	/// </summary>
	/// <param name="id">The ID of the chat to subscribe.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public async Task AddSubAsync(long id)
	{
		var exists = await _ctx.TgChats.FirstOrDefaultAsync(x => x.ChatId == id);
		if (exists == null)
		{
			await _ctx.AddAsync(new TgChatIdEntity { ChatId = id, SubsrcibedAt = DateTime.UtcNow });
			await _ctx.SaveChangesAsync();
		}
	}

	/// <summary>
	/// Asynchronously retrieves the collection of chat IDs for all subscribed Telegram chats.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of chat IDs
	/// for all subscribed chats.</returns>
	public async Task<IEnumerable<long>> GetSubsAsync() =>
		await _ctx.TgChats.AsNoTracking().Select(x => x.ChatId).ToListAsync();

	/// <summary>
	/// Asynchronously removes the chat subscription with the specified chat identifier, if it exists.
	/// </summary>
	/// <param name="id">The unique identifier of the chat subscription to remove.</param>
	/// <returns>A task that represents the asynchronous remove operation.</returns>
	public async Task RemoveSubAsync(long id)
	{
		var exists = await _ctx.TgChats.FirstOrDefaultAsync(x => x.ChatId == id);
		if (exists != null)
		{
			_ctx.Remove(exists);
			await _ctx.SaveChangesAsync();
		}
	}
}
