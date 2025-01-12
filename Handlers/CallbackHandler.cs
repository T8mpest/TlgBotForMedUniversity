using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotForMedUniversity.Data;
using TgBotForMedUniversity.Data.Models;

namespace TgBotForMedUniversity.Handlers
{
    public class CallbackHandler
    {
        private readonly TelegramBotClient _botClient;

        public CallbackHandler(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleCallbackAsync(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("answer_"))
            {
                // Обработка ответа
                var answerIndex = int.Parse(callbackQuery.Data.Replace("answer_", ""));
                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: $"Вы выбрали ответ: {answerIndex + 1}"
                );
            }
            else
            {
                switch (callbackQuery.Data)
                {
                    case "start_test":
                        await SendRandomQuestionAsync(callbackQuery);
                        break;

                    case "stop_test":
                        await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Тест завершён.");
                        break;

                    default:
                        await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Неизвестная команда.");
                        break;
                }
            }
        }

        private async Task SendRandomQuestionAsync(CallbackQuery callbackQuery)
        {
            using (var dbContext = new AppDbContext())
            {
                // Загружаем все вопросы из базы данных
                var questions = dbContext.Questions.AsEnumerable(); // Переключаемся на выполнение на стороне клиента

                // Выбираем случайный вопрос
                var question = questions.OrderBy(q => Guid.NewGuid()).FirstOrDefault();

                if (question == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "В базе данных нет вопросов."
                    );
                    return;
                }

                // Создаём инлайн-кнопки для каждого варианта ответа
                var buttons = question.Options
                    .Select((option, index) =>
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                            option,
                            $"answer_{index}"
                        )
                    ).ToArray();

                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons);

                // Отправляем вопрос с кнопками
                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: question.Text,
                    replyMarkup: keyboard
                );
            }
        }
    }
}
