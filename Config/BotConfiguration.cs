using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotForMedUniversity.Config
{
    public class BotConfiguration
    {
        public string BotToken { get; set; }

        public BotConfiguration()
        {
            // Токен можно хранить в appsettings.json или переменных окружения
            BotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? "7547543090:AAFI6CtKfHwRJoUyL6Ki5nz_sO5fYclKc1k";
        }
    }
}
