using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bkinator;
using NUnit.Framework;

namespace Bkinator_Test
{
    [TestFixture]
    public class BkinatorStateTest
    {
        [Test]
        public void SaveLoadTest()
        {
            var genie = new Genie(new Dictionary<string, AnswerStatistic>
                        {
                            {"2",
                                new AnswerStatistic
                                {
                                    AnswerCount = 2,
                                    AnsweredQuestionsById =
                                        new Dictionary<string, QuestionStatistic>
                                            {
                                                {"5", new QuestionStatistic {ChoicesFrequencies = new[] {1, 2, 3}}}
                                            }
                                }}
                        }, 3);
            var answers = new Dictionary<string, Answer>
                {
                    {"2", new Answer {Text = "A", Description = "B"}}
                };
            var questions = new Dictionary<string, Question>
                {
                    {"5", new Question {Text = "Q"}}
                };
            var memoryStream = new MemoryStream();
            (new BkinatorState(genie, answers, questions)).SaveToStream(memoryStream);
            memoryStream.Position = 0;
            var resultBkinatorState = BkinatorState.LoadFromStream(memoryStream);
            var field = typeof(Genie).GetField("answerStatistics", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            if (field == null)
                Assert.Fail();
            var resultingKnowledgeBase = (IDictionary<string, AnswerStatistic>)field.GetValue(resultBkinatorState.Genie);
            Assert.AreEqual(1, resultingKnowledgeBase.Count);
            Assert.AreEqual(2, resultingKnowledgeBase["2"].AnswerCount);
            Assert.AreEqual(1, resultingKnowledgeBase["2"].AnsweredQuestionsById.Count);
            Assert.AreEqual(3, resultingKnowledgeBase["2"].AnsweredQuestionsById["5"].ChoicesFrequencies.Length);
            Assert.AreEqual(1, resultingKnowledgeBase["2"].AnsweredQuestionsById["5"].ChoicesFrequencies[0]);
            Assert.AreEqual(2, resultingKnowledgeBase["2"].AnsweredQuestionsById["5"].ChoicesFrequencies[1]);
            Assert.AreEqual(3, resultingKnowledgeBase["2"].AnsweredQuestionsById["5"].ChoicesFrequencies[2]);
            Assert.AreEqual(1, resultBkinatorState.Answers.Count);
            Assert.AreEqual("A", resultBkinatorState.Answers["2"].Text);
            Assert.AreEqual("B", resultBkinatorState.Answers["2"].Description);
            Assert.AreEqual(1, resultBkinatorState.Questions.Count);
            Assert.AreEqual("Q", resultBkinatorState.Questions["5"].Text);
        }
    }
}
