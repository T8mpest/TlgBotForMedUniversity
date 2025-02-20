using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotForMedUniversity.Data.Models
{
    public class QuestionState
    {
        public int Id { get; set; } 
        public long ChatId { get; set; } 
        public int QuestionId { get; set; }
        public List<int> SelectedOptions { get; set; } = new List<int>();
    }
}
