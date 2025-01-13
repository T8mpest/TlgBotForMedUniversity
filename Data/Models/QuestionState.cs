using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotForMedUniversity.Data.Models
{
    public class QuestionState
    {
        public int Id { get; set; } // Уникальный идентификатор состояния
        public long ChatId { get; set; } // Идентификатор чата пользователя
        public int QuestionId { get; set; } // Идентификатор текущего вопроса
    }
}
