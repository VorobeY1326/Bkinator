using System;
using System.Collections.Generic;
using System.IO;
using Bkinator;

namespace WebKinator.State
{
    public static class InitialLoad
    {
        public static string StateFilePath = "state.bin";

        public static BkinatorState DefaultState = GetDefaultState();

        public static BkinatorState GetState()
        {
            try
            {
                var stateStream = new FileStream(StateFilePath, FileMode.Open);
                return BkinatorState.LoadFromStream(stateStream);
            }
            catch (Exception)
            {
                return DefaultState;
            }
        }

        private static BkinatorState GetDefaultState()
        {
            var answerStrings = new[] { "C#", "C" };
            var answerDescriptions = new[]
                {
                    "Объектно-ориентированный язык программирования. Разработан в 1998—2001 годах в компании Microsoft как язык разработки приложений для платформы Microsoft .NET Framework.",
                    "Стандартизированный процедурный язык программирования, разработанный в 1969—1973 годах сотрудниками Bell Labs. Си был создан для использования в операционной системе UNIX."
                };
            var answerIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var questionStrings = new[] { "Это ООП язык?" };
            var questionIds = new[] { Guid.NewGuid().ToString() };
            var answers = new Dictionary<string, Answer>
                {
                    {answerIds[0], new Answer {Text = answerStrings[0], Description = answerDescriptions[0]}},
                    {answerIds[1], new Answer {Text = answerStrings[1], Description = answerDescriptions[1]}}
                };
            var questions = new Dictionary<string, Question>
                {
                    {questionIds[0], new Question {Text = questionStrings[0]}}
                };
            var genie =
                new Genie(new List<AnswerStatistic>
                    {
                        new AnswerStatistic
                            {
                                AnswerId = answerIds[0],
                                AnswerCount = 1,
                                AnsweredQuestionsById =
                                    new Dictionary<string, QuestionStatistic>
                                        {
                                            {questionIds[0], new QuestionStatistic {ChoicesFrequencies = new[] {2, 1, 1, 1}}}
                                        }
                            },
                        new AnswerStatistic
                            {
                                AnswerId = answerIds[1],
                                AnswerCount = 1,
                                AnsweredQuestionsById =
                                    new Dictionary<string, QuestionStatistic>
                                        {
                                            {questionIds[0], new QuestionStatistic {ChoicesFrequencies = new[] {1, 2, 1, 1}}}
                                        }
                            },
                    }, 4);
            return new BkinatorState(genie, answers, questions);
        }
    }
}