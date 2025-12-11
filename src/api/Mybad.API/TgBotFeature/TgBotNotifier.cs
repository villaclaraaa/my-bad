using Mybad.Core.Services;
using Mybad.Storage.DB.Services;
using Telegram.Bot;

namespace Mybad.API.TgBotFeature;

public class TgBotNotifier : INotifier
{
	private readonly TelegramBotClient _bot;
	private readonly TgBotSubscriberService _tgService;

	public TgBotNotifier(TelegramBotClient bot, TgBotSubscriberService tgService)
	{
		_bot = bot;
		_tgService = tgService;
	}

	/// <summary>
	/// Sends message in Tg to all chats that are subscribed (got from TgService).
	/// </summary>
	/// <remarks>It does not handle exceptions for now. We dont actually care what happens after we try to send so whatever.</remarks>
	public async Task NotifyAsync(NotifyMessage message)
	{
		var chats = await _tgService.GetSubsAsync();
		foreach (var id in chats)
		{
			try
			{
				await _bot.SendMessage(id, message.ToString());
			}
			catch
			{
			}
		}
	}
}
