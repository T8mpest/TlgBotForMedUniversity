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

            if (context.Questions.Any())
                return;

            var questions = new[]
            {
                new Question
                {
                    Text = "Что такое H2O?",
                    Options = new[] { "Вода", "Кислород", "Углекислый газ", "Соль" },
                    CorrectAnswers = new[] { 0 }
                },
                new Question
                {
                    Text = "Выберите щелочи:",
                    Options = new[] { "NaOH", "HCl", "KOH", "CH3COOH" },
                    CorrectAnswers = new[] { 0, 2 }
                }
            };

            context.Questions.AddRange(questions);
            context.SaveChanges();
        }
    }
}
