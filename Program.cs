using Telegram.Bot;
using System;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TgBotForMedUniversity.Handlers;
using TgBotForMedUniversity.Data;
using TgBotForMedUniversity.Services;
using TgBotForMedUniversity.Config;

namespace TgBotForMedUniversity
{
    internal class Program
    {
        private static TelegramBotClient bot;

        static async Task Main(string[] args)
        {
            // 1. Инициализация базы данных
            using (var dbContext = new AppDbContext())
            {
                DbInitializer.Initialize(dbContext);
            }

            // 2. Настройка бота
            var botConfig = new BotConfiguration();
            var botClient = new TelegramBotClient(botConfig.BotToken);
            var botService = new BotService();
            botService.StartBot();

            // 3. Настройка обработчиков Telegram
            var updateHandler = new UpdateHandler(botClient);
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // Получаем все обновления
            };

            botClient.StartReceiving(
                updateHandler.HandleUpdateAsync,
                updateHandler.HandleErrorAsync,
                receiverOptions
            );

            Console.CancelKeyPress += (_, _) => Environment.Exit(0);

            // 4. Ожидание завершения программы
            await Task.Delay(-1);
        }
    }
}
