using System;
using System.Collections.Generic;
using System.IO;
using Bkinator;

namespace WebKinator.State
{
    public static class StateSaver
    {
        public static BkinatorState DefaultState = GetDefaultState();

        public static void SaveState(BkinatorState state, string stateFile)
        {
            using (var stateStream = new FileStream(stateFile, FileMode.OpenOrCreate))
            {
                state.SaveToStream(stateStream);
            }
        }

        public static BkinatorState GetState(string stateFile)
        {
            try
            {
                using (var stateStream = new FileStream(stateFile, FileMode.Open))
                {
                    return BkinatorState.LoadFromStream(stateStream);
                }
            }
            catch (Exception)
            {
                return DefaultState;
            }
        }

        private static BkinatorState GetDefaultState()
        {
            var answerStrings = new[] { "C#", "C", "Java", "C++" };
            var answerDescriptions = new[]
                {
                    "Объектно-ориентированный язык программирования. Разработан в 1998—2001 годах в компании Microsoft как язык разработки приложений для платформы Microsoft .NET Framework.",
                    "Стандартизированный процедурный язык программирования, разработанный в 1969—1973 годах сотрудниками Bell Labs. Си был создан для использования в операционной системе UNIX.",
                    "Объектно-ориентированный язык программирования, разработанный компанией Sun Microsystems. Приложения Java обычно компилируются в специальный байт-код, поэтому они могут работать на любой виртуальной Java-машине.",
                    "Компилируемый статически типизированный язык программирования общего назначения. Поддерживает такие парадигмы программирования как процедурное программирование, объектно-ориентированное программирование, обобщенное программирование."
                };
            var answerIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var questionStrings = new[] { "Это ООП язык?", "VM?" };
            var questionIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var answers = new Dictionary<string, Answer>
                {
                    {answerIds[0], new Answer {Text = answerStrings[0], Description = answerDescriptions[0]}},
                    {answerIds[1], new Answer {Text = answerStrings[1], Description = answerDescriptions[1]}},
                    {answerIds[2], new Answer {Text = answerStrings[2], Description = answerDescriptions[2]}},
                    {answerIds[3], new Answer {Text = answerStrings[3], Description = answerDescriptions[3]}}
                };
            var questions = new Dictionary<string, Question>
                {
                    {questionIds[0], new Question {Text = questionStrings[0]}},
                    {questionIds[1], new Question {Text = questionStrings[1]}}
                };
            var genie =
                new Genie(new Dictionary<string, AnswerStatistic>
                    {
                        {answerIds[0], new AnswerStatistic
                            {
                                AnswerCount = 1,
                                AnsweredQuestionsById =
                                    new Dictionary<string, QuestionStatistic>
                                        {
                                            {questionIds[0], new QuestionStatistic {ChoicesFrequencies = new[] {2, 1, 1, 1}}},
                                            {questionIds[1], new QuestionStatistic {ChoicesFrequencies = new[] {2, 1, 1, 1}}}
                                        }
                            }},
                            {answerIds[1], new AnswerStatistic
                            {
                                AnswerCount = 1,
                                AnsweredQuestionsById =
                                    new Dictionary<string, QuestionStatistic>
                                        {
                                            {questionIds[0], new QuestionStatistic {ChoicesFrequencies = new[] {1, 2, 1, 1}}},
                                            {questionIds[1], new QuestionStatistic {ChoicesFrequencies = new[] {1, 2, 1, 1}}}
                                        }
                            }},
                            {answerIds[2], new AnswerStatistic
                            {
                                AnswerCount = 1,
                                AnsweredQuestionsById =
                                    new Dictionary<string, QuestionStatistic>
                                        {
                                            {questionIds[0], new QuestionStatistic {ChoicesFrequencies = new[] {2, 1, 1, 1}}},
                                            {questionIds[1], new QuestionStatistic {ChoicesFrequencies = new[] {2, 1, 1, 1}}}
                                        }
                            }},
                            {answerIds[3], new AnswerStatistic
                            {
                                AnswerCount = 1,
                                AnsweredQuestionsById =
                                    new Dictionary<string, QuestionStatistic>
                                        {
                                            {questionIds[0], new QuestionStatistic {ChoicesFrequencies = new[] {2, 1, 1, 1}}},
                                            {questionIds[1], new QuestionStatistic {ChoicesFrequencies = new[] {1, 2, 1, 1}}}
                                        }
                            }}
                    }, 4);
            return new BkinatorState(genie, answers, questions);
        }
    }
}