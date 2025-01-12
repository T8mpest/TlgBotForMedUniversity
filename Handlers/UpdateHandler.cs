using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgBotForMedUniversity.Handlers
{
    public class UpdateHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly CommandHandler _commandHandler;
        private readonly CallbackHandler _callbackHandler;

        public UpdateHandler(TelegramBotClient botClient)
        {
            _botClient = botClient;
            _commandHandler = new CommandHandler(botClient);
            _callbackHandler = new CallbackHandler(botClient);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message != null)
            {
                await _commandHandler.HandleCommandAsync(update.Message);
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                await _callbackHandler.HandleCallbackAsync(update.CallbackQuery);
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
