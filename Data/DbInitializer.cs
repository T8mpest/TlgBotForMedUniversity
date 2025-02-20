using System.Linq;
using TgBotForMedUniversity.Data;
using TgBotForMedUniversity.Data.Models;

namespace TgBotForMedUniversity.Data
{
    internal static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.QuestionStates.Any())
            {
                var states = new[]
                {
            new QuestionState
            {
                Id = 1,
                ChatId = 123456789,
                QuestionId = 1,
            }
        };

                context.QuestionStates.AddRange(states);
                context.SaveChanges();
            }
        }

    }
}
