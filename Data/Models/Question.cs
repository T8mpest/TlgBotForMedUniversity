using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotForMedUniversity.Data.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string[] Options { get; set; }
        public int[] CorrectAnswers { get; set; }
    }
}
