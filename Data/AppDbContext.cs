using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TgBotForMedUniversity.Data.Models;

namespace TgBotForMedUniversity.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Указываем путь к файлу базы данных SQLite
            optionsBuilder.UseSqlite("Data Source=questions.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация для преобразования массивов в строку
            modelBuilder.Entity<Question>()
                .Property(q => q.Options)
                .HasConversion(
                    options => string.Join(";", options),    // Сериализация в строку
                    options => options.Split(new[] { ';' }, StringSplitOptions.None) // Явный вызов
                );

            modelBuilder.Entity<Question>()
                .Property(q => q.CorrectAnswers)
                .HasConversion(
                    answers => string.Join(",", answers),
                    answers => answers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()
                );
        }
    }
}
