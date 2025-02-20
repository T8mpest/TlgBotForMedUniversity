using Microsoft.EntityFrameworkCore;
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
                var selectedAnswerIndex = int.Parse(callbackQuery.Data.Replace("answer_", ""));
                await ToggleAnswerAsync(callbackQuery, selectedAnswerIndex);
            }
            else if (callbackQuery.Data == "submit_answers")
            {
                await CheckAnswersAsync(callbackQuery);
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
                var question = dbContext.Questions
                    .OrderBy(q => EF.Functions.Random())
                    .FirstOrDefault();

                if (question == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "No more questions in database."
                    );
                    return;
                }

                var questionState = new QuestionState
                {
                    ChatId = callbackQuery.Message.Chat.Id,
                    QuestionId = question.Id,
                    SelectedOptions = new List<int>()
                };

                dbContext.QuestionStates.Add(questionState);
                dbContext.SaveChanges();

                string optionsText = string.Join("\n", question.Options.Select((option, index) =>
                    $"{(char)('A' + index)}) {option}"
                ));

                var buttons = question.Options
                    .Select((_, index) =>
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                            $"{(char)('A' + index)}",
                            $"answer_{index}"
                        )
                    ).ToList();

                buttons.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                    "Send Answers",
                    "submit_answers"
                ));

                var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons.Chunk(4));

                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: $"{question.Text}\n\n{optionsText}",
                    replyMarkup: keyboard
                );
            }
        }



        private async Task DisplayQuestionAsync(CallbackQuery callbackQuery, Question question, List<int> selectedOptions)
        {
            string optionsText = string.Join("\n", question.Options.Select((option, index) =>
                $"{(selectedOptions.Contains(index) ? "✅" : "")} {(char)('A' + index)}) {option}"
            ));

            var buttons = question.Options
                .Select((_, index) =>
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                        $"{(selectedOptions.Contains(index) ? "✅ " : "")}{(char)('A' + index)}",
                        $"answer_{index}"
                    )
                ).ToList();

            buttons.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                "Send Answers",
                "submit_answers"
            ));

            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons.Chunk(4)); // Делаем по 4 кнопки в строке

            await _botClient.EditMessageTextAsync(
                chatId: callbackQuery.Message.Chat.Id,
                messageId: callbackQuery.Message.MessageId,
                text: $"{question.Text}\n\n{optionsText}",
                replyMarkup: keyboard
            );
        }



        private async Task ToggleAnswerAsync(CallbackQuery callbackQuery, int selectedAnswerIndex)
        {
            using (var dbContext = new AppDbContext())
            {
                var questionState = dbContext.QuestionStates.FirstOrDefault(qs => qs.ChatId == callbackQuery.Message.Chat.Id);

                if (questionState == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Error: No active question found."
                    );
                    return;
                }

                var selectedOptions = questionState.SelectedOptions;

                if (selectedOptions.Contains(selectedAnswerIndex))
                    selectedOptions.Remove(selectedAnswerIndex); // Unselect
                else
                    selectedOptions.Add(selectedAnswerIndex); // Select

                questionState.SelectedOptions = selectedOptions;
                dbContext.QuestionStates.Update(questionState);
                dbContext.SaveChanges();

                var currentQuestion = dbContext.Questions.FirstOrDefault(q => q.Id == questionState.QuestionId);

                await DisplayQuestionAsync(callbackQuery, currentQuestion, selectedOptions);
            }
        }



        private async Task CheckAnswersAsync(CallbackQuery callbackQuery)
        {
            using (var dbContext = new AppDbContext())
            {
                var questionState = dbContext.QuestionStates
                    .FirstOrDefault(qs => qs.ChatId == callbackQuery.Message.Chat.Id);

                if (questionState == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Error: No active question found."
                    );
                    return;
                }

                var currentQuestion = dbContext.Questions.FirstOrDefault(q => q.Id == questionState.QuestionId);

                if (currentQuestion == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Error: Question not found."
                    );
                    return;
                }

                var correctAnswers = currentQuestion.CorrectAnswers;
                var selectedAnswers = questionState.SelectedOptions;

                int correctCount = selectedAnswers.Count(sa => correctAnswers.Contains(sa));
                int incorrectCount = selectedAnswers.Count(sa => !correctAnswers.Contains(sa));

                var correctLetters = correctAnswers
                    .Select(index => $"{(char)('A' + index)}")
                    .ToList();

                string resultMessage = $"Correct answers: {correctCount}\n" +
                                       $"Incorrect answers: {incorrectCount}\n" +
                                       $"Correct was: {string.Join(", ", correctLetters)}";

                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: resultMessage
                );

                dbContext.QuestionStates.Remove(questionState);
                dbContext.SaveChanges();

                
                await _botClient.EditMessageReplyMarkupAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    replyMarkup: null
                );

                await SendRandomQuestionAsync(callbackQuery);
            }
        }





    }
}
