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
            
            using (var dbContext = new AppDbContext())
            {
                dbContext.Database.Migrate();
            }

           
            var botConfig = new BotConfiguration();
            var botClient = new TelegramBotClient(botConfig.BotToken);
            var botService = new BotService();
            botService.StartBot();

           
            var updateHandler = new UpdateHandler(botClient);
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() 
            };

            botClient.StartReceiving(
                updateHandler.HandleUpdateAsync,
                updateHandler.HandleErrorAsync,
                receiverOptions
            );

            Console.CancelKeyPress += (_, _) => Environment.Exit(0);

            
            await Task.Delay(-1);
        }
    }
}
