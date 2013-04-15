using System;
using System.Collections.Generic;
using System.Linq;

namespace Bkinator
{
    /*
     * 
     * Algorithm is based on Bayes's theorem and on information entropy.
     * More about ideas used in this solution you can read here ( on russian ):
     * http://habrahabr.ru/post/84364/ ( author isn't me )
     * 
     */
    public class Genie
    {
        private readonly IList<AnswerStatistic> knowledgeBase;
        private readonly IDictionary<string, QuestionStatistic> questionsSummaryStatistics; 
        private readonly IList<string> questionIds; 
        private readonly double answersGuessedCount;
        private readonly double questionsCount;
        private readonly int answeringChoicesCount;

        public Genie(IList<AnswerStatistic> knowledgeBase, int questionsCount, int answeringChoicesCount)
        {
            this.knowledgeBase = knowledgeBase;
            this.questionsCount = questionsCount;
            this.answeringChoicesCount = answeringChoicesCount;
            answersGuessedCount = knowledgeBase.Sum(s => s.AnswerCount);
            questionsSummaryStatistics = CountQuestionsSummaryStatistics();
            questionIds = knowledgeBase.SelectMany(s => s.AnsweredQuestionsById.Keys).Distinct().ToList();
        }

        private IDictionary<string, QuestionStatistic> CountQuestionsSummaryStatistics()
        {
            return knowledgeBase.SelectMany(a => a.AnsweredQuestionsById).GroupBy(p => p.Key)
                                .ToDictionary(g => g.Key, g => new QuestionStatistic
                                    {
                                        ChoicesFrequencies = g.Aggregate(new int[answeringChoicesCount], (curr, q) =>
                                            {
                                                for (int i = 0; i < answeringChoicesCount; i++)
                                                {
                                                    curr[i] += q.Value.ChoicesFrequencies[i];
                                                }
                                                return curr;
                                            })
                                    });
        }

        public Tuple<IEnumerable<AnswerGuess>,string> GetTopGuessesAndNextQuestionId(IList<AnsweredQuestion> answeredQuestions)
        {
            var answerGuesses = new List<AnswerGuess>();
            double answeredQuestionsProbability = 0;
            foreach (var answerStatistic in knowledgeBase)
            {
                var answeredQuestionsProbabilityRelativeToAnswer = answeredQuestions.Aggregate(1.0,
                    (curr, q) => curr * GetAnsweringFrequency(answerStatistic, q) / GetTotalAnsweringFrequency(answerStatistic, q));
                var answerProbability = answeredQuestionsProbabilityRelativeToAnswer * answerStatistic.AnswerCount / answersGuessedCount;
                answerGuesses.Add(new AnswerGuess
                    {
                        AnswerId = answerStatistic.AnswerId,
                        Probability = answerProbability,
                        AnswerStatistic = answerStatistic
                    });
                answeredQuestionsProbability += answerProbability;
            }
            foreach (var answerGuess in answerGuesses)
            {
                answerGuess.Probability /= answeredQuestionsProbability;
            }

            string nextQuestionId = null;
            double minQuestionEntropy = double.MaxValue;
            var answeredQuestionIds = new HashSet<string>(answeredQuestions.Select(q => q.QuestionId));
            //TODO Not all answers, top N only? For speed.
            foreach (var questionId in questionIds)
            {
                if (answeredQuestionIds.Contains(questionId))
                    continue;
                double entropy = 0.0;
                for (int i = 0; i < answeringChoicesCount; i++)
                {
                    var answeredQuestion = new AnsweredQuestion {Choise = i, QuestionId = questionId};
                    foreach (var answerGuess in answerGuesses)
                    {
                        double probability = answerGuess.Probability *
                            GetAnsweringFrequency(answerGuess.AnswerStatistic, answeredQuestion) /
                            GetTotalAnsweringFrequency(answerGuess.AnswerStatistic, answeredQuestion);
                        entropy +=  - probability*Math.Log(probability, 2.0);
                    }
                }
                if (entropy < minQuestionEntropy)
                {
                    minQuestionEntropy = entropy;
                    nextQuestionId = questionId;
                }
            }

            return new Tuple<IEnumerable<AnswerGuess>, string>(answerGuesses.OrderByDescending(g => g.Probability), nextQuestionId);
        }

        private int GetAnsweringFrequency(AnswerStatistic answerStatistic, AnsweredQuestion answeredQuestion)
        {
            if (!answerStatistic.AnsweredQuestionsById.ContainsKey(answeredQuestion.QuestionId))
                return 1;
            return answerStatistic.AnsweredQuestionsById[answeredQuestion.QuestionId].ChoicesFrequencies[answeredQuestion.Choise];
        }


        private int GetTotalAnsweringFrequency(AnswerStatistic answerStatistic, AnsweredQuestion answeredQuestion)
        {
            if (!answerStatistic.AnsweredQuestionsById.ContainsKey(answeredQuestion.QuestionId))
                return answeringChoicesCount;
            return answerStatistic.AnsweredQuestionsById[answeredQuestion.QuestionId].ChoicesFrequenciesTotal;
        }
    }
}
