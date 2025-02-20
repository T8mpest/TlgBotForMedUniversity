using Telegram.Bot;
using System;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TgBotForMedUniversity.Handlers;
using TgBotForMedUniversity.Data;
using TgBotForMedUniversity.Services;
using TgBotForMedUniversity.Config;
using Microsoft.EntityFrameworkCore;

namespace TgBotForMedUniversity
{
    internal class Program
    {
        private static TelegramBotClient bot;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Migrating database...");
            using (var dbContext = new AppDbContext())
            {
                dbContext.Database.Migrate();
            }
            Console.WriteLine("Database migrated successfully.");

            Console.WriteLine("Configuring bot...");
            var botConfig = new BotConfiguration();
            bot = new TelegramBotClient(botConfig.BotToken);

            var botInfo = await bot.GetMeAsync();
            Console.WriteLine($"Bot started: @{botInfo.Username}");

            var botService = new BotService();
            botService.StartBot();

            var updateHandler = new UpdateHandler(bot);
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            Console.WriteLine("Starting to receive updates...");
            bot.StartReceiving(
                updateHandler.HandleUpdateAsync,
                updateHandler.HandleErrorAsync,
                receiverOptions
            );

            Console.WriteLine("Bot is running. Press Ctrl+C to exit.");

            Console.CancelKeyPress += (_, _) => Environment.Exit(0);

            await Task.Delay(-1);
        }
    }
}
