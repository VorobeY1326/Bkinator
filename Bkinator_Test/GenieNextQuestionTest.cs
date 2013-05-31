using System.Collections.Generic;
using Bkinator;
using NUnit.Framework;

namespace Bkinator_Test
{
    [TestFixture]
    public class GenieNextQuestionTest
    {
        [Test]
        public void LastQuestionRemainsTest()
        {
            var knowledgeBase = new Dictionary<string, AnswerStatistic>
                {
                    {"1", new AnswerStatistic
                        {
                            AnswerCount = 1,
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"1", new QuestionStatistic {ChoicesFrequencies = new[] {5, 1}}},
                                    {"2", new QuestionStatistic {ChoicesFrequencies = new[] {1, 1}}}
                                }
                        }}
                };
            var genie = new Genie(knowledgeBase, 2);
            var answeredQuestions = new List<AnsweredQuestion>
                {
                    new AnsweredQuestion {Choise = 1, QuestionId = "1"}
                };
            var nextQuestionId = genie.GetNextQuestionId(answeredQuestions, genie.GetAnswerGuesses(answeredQuestions));
            Assert.AreEqual("2", nextQuestionId);
        }

        [Test]
        public void NoQuestionsAskedTest()
        {
            var knowledgeBase = new Dictionary<string, AnswerStatistic>
                {
                    {"1", new AnswerStatistic
                        {
                            AnswerCount = 1,
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"1", new QuestionStatistic {ChoicesFrequencies = new[] {1, 1}}},
                                    {"2", new QuestionStatistic {ChoicesFrequencies = new[] {1, 5}}}
                                }
                        }},
                    {"2", new AnswerStatistic
                        {
                            AnswerCount = 1,
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"1", new QuestionStatistic {ChoicesFrequencies = new[] {1, 1}}},
                                    {"2", new QuestionStatistic {ChoicesFrequencies = new[] {1, 1}}}
                                }
                        }}
                };
            var genie = new Genie(knowledgeBase, 2);
            var answeredQuestions = new List<AnsweredQuestion>();
            var nextQuestionId = genie.GetNextQuestionId(answeredQuestions, genie.GetAnswerGuesses(answeredQuestions));
            Assert.AreEqual("2", nextQuestionId);
        }

        [Test]
        public void NoQuestionsRemainedTest()
        {
            var knowledgeBase = new Dictionary<string, AnswerStatistic>
                {
                    {"1", new AnswerStatistic
                        {
                            AnswerCount = 1,
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"1", new QuestionStatistic {ChoicesFrequencies = new[] {1, 1}}}
                                }
                        }}
                };
            var genie = new Genie(knowledgeBase, 2);
            var answeredQuestions = new List<AnsweredQuestion> {new AnsweredQuestion {Choise = 1, QuestionId = "1"}};
            var nextQuestionId = genie.GetNextQuestionId(answeredQuestions, genie.GetAnswerGuesses(answeredQuestions));
            Assert.AreEqual(null, nextQuestionId);
        }

        [Test]
        public void SimpleEntropyTest()
        {
            var knowledgeBase = new Dictionary<string, AnswerStatistic>
                {
                    {
                        "1", new AnswerStatistic
                            {
                                AnswerCount = 1,
                                AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                    {
                                        {"1", new QuestionStatistic {ChoicesFrequencies = new[] {2, 2, 1}}},
                                        {"2", new QuestionStatistic {ChoicesFrequencies = new[] {1, 2, 2}}},
                                        {"3", new QuestionStatistic {ChoicesFrequencies = new[] {1, 10, 1}}},
                                    }
                            }
                    },
                    {
                        "2", new AnswerStatistic
                            {
                                AnswerCount = 1,
                                AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                    {
                                        {"1", new QuestionStatistic {ChoicesFrequencies = new[] {1, 1, 1}}}
                                    }
                            }
                    }
                };
            var genie = new Genie(knowledgeBase, 3);
            var answeredQuestions = new List<AnsweredQuestion> {new AnsweredQuestion {Choise = 1, QuestionId = "2"}};
            var nextQuestionId = genie.GetNextQuestionId(answeredQuestions, genie.GetAnswerGuesses(answeredQuestions));
            Assert.AreEqual("3", nextQuestionId);
        }

        [Test]
        public void ComplexEntropyTest()
        {
            var knowledgeBase = new Dictionary<string, AnswerStatistic>
                {
                    {
                        "1", new AnswerStatistic
                            {
                                AnswerCount = 1,
                                AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                    {
                                        {"1", new QuestionStatistic {ChoicesFrequencies = new[] {1, 5, 1, 1}}},
                                        {"2", new QuestionStatistic {ChoicesFrequencies = new[] {1, 1, 1, 1}}},
                                        {"3", new QuestionStatistic {ChoicesFrequencies = new[] {1, 5, 1, 1}}}
                                    }
                            }
                    },
                    {
                        "2", new AnswerStatistic
                            {
                                AnswerCount = 1,
                                AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                    {
                                        {"1", new QuestionStatistic {ChoicesFrequencies = new[] {1, 1, 1, 1}}},
                                        {"2", new QuestionStatistic {ChoicesFrequencies = new[] {5, 1, 1, 1}}},
                                        {"3", new QuestionStatistic {ChoicesFrequencies = new[] {5, 1, 1, 1}}}
                                    }
                            }
                    }
                };
            var genie = new Genie(knowledgeBase, 4);
            var answeredQuestions = new List<AnsweredQuestion>();
            var nextQuestionId = genie.GetNextQuestionId(answeredQuestions, genie.GetAnswerGuesses(answeredQuestions));
            Assert.AreEqual("3", nextQuestionId);
        }
    }
}
