using Microsoft.EntityFrameworkCore;
using TgBotForMedUniversity.Data.Models;

namespace TgBotForMedUniversity.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionState> QuestionStates { get; set; } 


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), "questions.db"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Question>()
                .Property(q => q.Options)
                .HasConversion(
                    options => string.Join(";", options), 
                    options => options.Split(new[] { ';' }, StringSplitOptions.None) 
                );

            modelBuilder.Entity<Question>()
                .Property(q => q.CorrectAnswers)
                .HasConversion(
                    answers => string.Join(",", answers),
                    answers => answers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray() 
                );
            modelBuilder.Entity<QuestionState>()
                 .Property(qs => qs.SelectedOptions)
                 .HasConversion(
                       options => string.Join(",", options),
                       options => options.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );
        }
    }
}
