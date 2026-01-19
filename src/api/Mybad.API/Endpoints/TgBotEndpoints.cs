using Mybad.Storage.DB.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Mybad.API.Endpoints;

public static class TgBotEndpoints
{
	public static RouteGroupBuilder MapTgBotEndpoints(this IEndpointRouteBuilder routes, string webhookUrl)
	{
		var group = routes.MapGroup("tgbot")
			.WithTags("TelegramBot");

		group.MapGet("setwebhook", async (TelegramBotClient bot, CancellationToken token = default) => await SetWebHook(bot, webhookUrl, token))
			.AddEndpointFilter<ApiKeyEndpointFilter>();

		group.MapPost("", HandleUpdate)
			.ExcludeFromDescription();

		return group;
	}

	private static async Task<IResult> SetWebHook(TelegramBotClient bot, string webhookUrl, CancellationToken token)
	{
		await bot.SetWebhook(webhookUrl, allowedUpdates: [UpdateType.Message], dropPendingUpdates: true, cancellationToken: token);
		return TypedResults.Ok($"Webhook set to {webhookUrl}.");
	}

	private static async Task HandleUpdate(Update update, TelegramBotClient bot, TgBotSubscriberService botService)
	{
		if (update.Message is null) return;         // we want only updates about new Message
		if (update.Message.Text is null) return;    // we want only updates about new Text Message

		if (update.Message.Entities != null     // we want only updates with Bot commands list
			&& update.Message.Entities.FirstOrDefault(x => x.Type == MessageEntityType.BotCommand) is null) return;

		var command = update.Message.Text[1..];
		string cmd = command.Trim().ToLowerInvariant();
		var id = update.Message.Chat.Id;

		switch (cmd)
		{
			case "sub_matchupcacher":
				await botService.AddSubAsync(id);
				await bot.SendMessage(id, "GJ! You will now receive messages.");
				break;

			case "unsub_matchupcacher":
				await botService.RemoveSubAsync(id);
				await bot.SendMessage(id, "Bruh! You will not receive updates anymore.");
				break;

			case "status":
				await bot.SendMessage(id, "WIP");
				break;
		}
	}
}
