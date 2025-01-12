using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBotForMedUniversity.Handlers
{
    public class CommandHandler
    {
        private readonly TelegramBotClient _botClient;

        public CommandHandler(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleCommandAsync(Message message)
        {
            switch (message.Text)
            {
                case "/start":
                    await _botClient.SendTextMessageAsync(
                        message.Chat.Id,
                        "Добро пожаловать! Нажмите 'Старт тест', чтобы начать.",
                        replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                            new[]
                            {
                                new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Старт тест", "start_test") },
                                new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Стоп тест", "stop_test") }
                            }
                        )
                    );
                    break;

                case "/stop":
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Тест завершён!");
                    break;

                default:
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда.");
                    break;
            }
        }
    }
}
