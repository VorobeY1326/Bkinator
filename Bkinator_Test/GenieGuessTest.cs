using System.Collections.Generic;
using Bkinator;
using NUnit.Framework;
using System.Linq;

namespace Bkinator_Test
{
    [TestFixture]
    public class GenieGuessTest
    {

        private Genie genie;

        private void SetUpSimple()
        {
            genie = new Genie(new List<AnswerStatistic>
                {
                    new AnswerStatistic
                        {
                            AnswerCount = 1,
                            AnswerId = "1",
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"1", new QuestionStatistic {ChoicesFrequencies = new[] {2, 1}}}
                                }
                        },
                    new AnswerStatistic
                        {
                            AnswerCount = 1,
                            AnswerId = "2",
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"1", new QuestionStatistic {ChoicesFrequencies = new[] {1, 2}}}
                                }
                        }
                }, 2);
        }

        private void SetUpComplex()
        {
            genie = new Genie(new List<AnswerStatistic>
                {
                    new AnswerStatistic
                        {
                            AnswerCount = 3,
                            AnswerId = "1",
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"1", new QuestionStatistic {ChoicesFrequencies = new[] {5, 1}}}
                                }
                        },
                    new AnswerStatistic
                        {
                            AnswerCount = 2,
                            AnswerId = "2",
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"3", new QuestionStatistic {ChoicesFrequencies = new[] {1, 5}}}
                                }
                        },
                    new AnswerStatistic
                        {
                            AnswerCount = 2,
                            AnswerId = "3",
                            AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>
                                {
                                    {"1", new QuestionStatistic {ChoicesFrequencies = new[] {5, 1}}},
                                    {"2", new QuestionStatistic {ChoicesFrequencies = new[] {5, 1}}},
                                    {"3", new QuestionStatistic {ChoicesFrequencies = new[] {5, 1}}}
                                }
                        }
                }, 2);
        }

        [Test]
        public void SimpleTest()
        {
            SetUpSimple();
            var answer = genie.GetAnswerGuesses(new List<AnsweredQuestion> {new AnsweredQuestion {Choise = 0, QuestionId = "1"}});
            Assert.AreEqual(2, answer.Count);
            Assert.AreEqual("1", answer[0].AnswerId);
            Assert.AreEqual("2", answer[1].AnswerId);
            Assert.Greater(answer[0].Probability, 0.6);
            Assert.Less(answer[1].Probability, 0.4);
            TestSumIs1(answer);
        }

        [Test]
        public void NoQuestionsAskedTest()
        {
            SetUpSimple();
            var answer = genie.GetAnswerGuesses(new List<AnsweredQuestion>()).ToList();
            Assert.AreEqual(2, answer.Count);
            Assert.AreEqual(answer[0].Probability, 0.5, 1e-5);
            Assert.AreEqual(answer[1].Probability, 0.5, 1e-5);
            TestSumIs1(answer);
        }

        [Test]
        public void ComplexFirstQuestionTest()
        {
            SetUpComplex();
            var answer =
                genie.GetAnswerGuesses(new List<AnsweredQuestion> { new AnsweredQuestion { Choise = 0, QuestionId = "1" } });
            Assert.AreEqual(3, answer.Count);
            Assert.AreEqual("1", answer[0].AnswerId);
            Assert.Greater(answer[0].Probability, 0.4);
            TestSumIs1(answer);
        }

        [Test]
        public void ComplexLastQuestionTest()
        {
            SetUpComplex();
            var answer =
                genie.GetAnswerGuesses(new List<AnsweredQuestion> {new AnsweredQuestion {Choise = 1, QuestionId = "3"}});
            Assert.AreEqual(3, answer.Count);
            Assert.AreEqual("2", answer[0].AnswerId);
            Assert.Greater(answer[0].Probability, 0.4);
            TestSumIs1(answer);
        }

        [Test]
        public void ComplexNoQuestionsTest()
        {
            SetUpComplex();
            var answer = genie.GetAnswerGuesses(new List<AnsweredQuestion> ());
            Assert.AreEqual(3, answer.Count);
            Assert.AreEqual("1", answer[0].AnswerId);
            Assert.AreEqual("2", answer[1].AnswerId);
            Assert.AreEqual("3", answer[2].AnswerId);
            Assert.GreaterOrEqual(answer[0].Probability, 0.4);
            TestSumIs1(answer);
        }

        [Test]
        public void ComplexThreeQuestionsTest()
        {
            SetUpComplex();
            var answer =
                genie.GetAnswerGuesses(new List<AnsweredQuestion>
                    {
                        new AnsweredQuestion { Choise = 0, QuestionId = "1" },
                        new AnsweredQuestion { Choise = 0, QuestionId = "2" },
                        new AnsweredQuestion { Choise = 0, QuestionId = "3" }
                    });
            Assert.AreEqual(3, answer.Count);
            Assert.AreEqual("3", answer[0].AnswerId);
            Assert.Greater(answer[0].Probability, 0.6);
            TestSumIs1(answer);
        }

        private void TestSumIs1(IEnumerable<AnswerGuess> guesses)
        {
            Assert.AreEqual(1.0, guesses.Sum(g => g.Probability), 1e-5);
        }
    }
}
