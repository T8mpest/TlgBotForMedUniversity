﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotForMedUniversity.Data;

namespace TgBotForMedUniversity.Services
{
    public class BotService
    {
        public BotService() 
        {
            using (var dbContext = new AppDbContext())
            {
                DbInitializer.Initialize(dbContext);
            }
        }
        public void StartBot()
        {
            Console.WriteLine("Bot Started");
        }
    }
}
