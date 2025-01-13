﻿using System;
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
                // Обработка ответа пользователя
                var selectedAnswerIndex = int.Parse(callbackQuery.Data.Replace("answer_", ""));
                await HandleAnswerAsync(callbackQuery, selectedAnswerIndex);
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
                // Переключаем запрос на клиентскую часть для поддержки случайного выбора
                var questions = dbContext.Questions.AsEnumerable();
                var question = questions.OrderBy(q => Guid.NewGuid()).FirstOrDefault();

                if (question == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "В базе данных нет вопросов."
                    );
                    return;
                }

                var buttons = question.Options
                    .Select((option, index) =>
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                            option,
                            $"answer_{index}"
                        )
                    ).ToArray();

                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons);

                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: question.Text,
                    replyMarkup: keyboard
                );

                // Сохраняем текущий вопрос для обработки ответа
                dbContext.QuestionStates.Add(new QuestionState
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    QuestionId = question.Id
                });
                dbContext.SaveChanges();
            }
        }

        private async Task HandleAnswerAsync(CallbackQuery callbackQuery, int selectedAnswerIndex)
        {
            using (var dbContext = new AppDbContext())
            {
                // Получаем текущий вопрос из состояния
                var questionState = dbContext.QuestionStates.FirstOrDefault(qs => qs.ChatId == callbackQuery.Message.Chat.Id);

                if (questionState == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Произошла ошибка. Текущий вопрос не найден."
                    );
                    return;
                }

                var currentQuestion = dbContext.Questions.FirstOrDefault(q => q.Id == questionState.QuestionId);

                if (currentQuestion == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Произошла ошибка. Вопрос не найден."
                    );
                    return;
                }

                // Проверяем, правильный ли ответ
                bool isCorrect = currentQuestion.CorrectAnswers.Contains(selectedAnswerIndex);

                string responseMessage = isCorrect
                    ? "Ваш ответ правильный!"
                    : "Ваш ответ неправильный. Правильный ответ: " + string.Join(", ", currentQuestion.CorrectAnswers.Select(i => currentQuestion.Options[i]));

                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: responseMessage
                );

                // Удаляем состояние текущего вопроса
                dbContext.QuestionStates.Remove(questionState);
                dbContext.SaveChanges();

                // Переход к следующему вопросу
                await SendRandomQuestionAsync(callbackQuery);
            }
        }
    }
}
