using Microsoft.EntityFrameworkCore;
using TgBotForMedUniversity.Data.Models;

namespace TgBotForMedUniversity.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionState> QuestionStates { get; set; } // Добавляем QuestionState

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=questions.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация для преобразования массивов в строки и обратно
            modelBuilder.Entity<Question>()
                .Property(q => q.Options)
                .HasConversion(
                    options => string.Join(";", options), // Преобразуем массив в строку
                    options => options.Split(new[] { ';' }, StringSplitOptions.None) // Преобразуем строку обратно в массив
                );

            modelBuilder.Entity<Question>()
                .Property(q => q.CorrectAnswers)
                .HasConversion(
                    answers => string.Join(",", answers), // Преобразуем массив в строку
                    answers => answers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray() // Преобразуем строку обратно в массив
                );
        }
    }
}
