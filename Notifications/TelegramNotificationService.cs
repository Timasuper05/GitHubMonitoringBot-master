using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GitHub_Monitoring_Bot;

public class TelegramNotificationService(
    AppSettings settings,
    TelegramBotClient bot,
    ILogger<TelegramNotificationService> logger) : INotification
{
    public async Task SendMessageAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        var replyMarkup = BuildReplyMarkup(message);

        if (settings.ThreadId is > 0)
        {
            await bot.SendMessage(
                settings.ChatId,
                message.Text,
                cancellationToken: cancellationToken,
                replyMarkup: replyMarkup,
                messageThreadId: settings.ThreadId);

            logger.LogInformation("Telegram message was sent to chat {ChatId} thread {ThreadId}", settings.ChatId, settings.ThreadId);
            return;
        }

        await bot.SendMessage(
            settings.ChatId,
            message.Text,
            cancellationToken: cancellationToken,
            replyMarkup: replyMarkup);
        logger.LogInformation("Telegram message was sent to chat {ChatId}", settings.ChatId);
    }

    private static InlineKeyboardMarkup? BuildReplyMarkup(NotificationMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.ButtonText) ||
            string.IsNullOrWhiteSpace(message.ButtonUrl))
        {
            return null;
        }

        return new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(message.ButtonText, message.ButtonUrl));
    }
}
