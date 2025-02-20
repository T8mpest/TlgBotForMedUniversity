using Microsoft.Extensions.Configuration;
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
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<BotConfiguration>();

            var configuration = builder.Build();
            BotToken = configuration["BotConfiguration:BotToken"];
        }
    }
}
