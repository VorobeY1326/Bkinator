using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace Bkinator
{
    /*
     * 
     * Algorithm is based on Bayes's theorem and on information entropy.
     * More about ideas used in this solution you can read here ( on russian ):
     * http://habrahabr.ru/post/84364/ ( author isn't me )
     * 
     */
    [ProtoContract]
    public class Genie
    {
        [ProtoMember(1)]
        private readonly IDictionary<string, AnswerStatistic> answerStatistics;
        [ProtoMember(2)]
        private readonly IDictionary<string, QuestionStatistic> questionStatistics;
        [ProtoMember(3)]
        private int answersGuessedCount;
        [ProtoMember(4)]
        private readonly int answeringChoicesCount;

        private const int maxGuesses = 50;

// ReSharper disable UnusedMember.Local
        private Genie()
        {}
// ReSharper restore UnusedMember.Local

        public Genie(IDictionary<string, AnswerStatistic> answerStatistics, int answeringChoicesCount)
        {
            this.answerStatistics = answerStatistics;
            this.answeringChoicesCount = answeringChoicesCount;
            answersGuessedCount = answerStatistics.Sum(s => s.Value.AnswerCount);
            questionStatistics = answerStatistics.SelectMany(s => s.Value.AnsweredQuestionsById)
                                    .GroupBy(p => p.Key)
                                    .ToDictionary(g => g.Key, g => new QuestionStatistic
                                        {
                                            ChoicesFrequencies = g.Aggregate(new int[answeringChoicesCount], (curr, p) =>
                                                {
                                                    for (int i = 0; i < answeringChoicesCount; i++)
                                                    {
                                                        curr[i] += p.Value.ChoicesFrequencies[i];
                                                    }
                                                    return curr;
                                                })
                                        });
        }

        public IList<AnswerGuess> GetAnswerGuesses(IList<AnsweredQuestion> answeredQuestions)
        {
            var answerGuesses = new List<AnswerGuess>();
            double answeredQuestionsProbability = 0;
            foreach (var answerStatistic in answerStatistics)
            {
                var answeredQuestionsProbabilityRelativeToAnswer = answeredQuestions.Aggregate(1.0,
                    (curr, q) => curr * GetAnsweringFrequency(answerStatistic.Value, q) / GetTotalAnsweringFrequency(answerStatistic.Value, q));
                var answerProbability = answeredQuestionsProbabilityRelativeToAnswer * answerStatistic.Value.AnswerCount / answersGuessedCount;
                answerGuesses.Add(new AnswerGuess
                    {
                        AnswerId = answerStatistic.Key,
                        Probability = answerProbability,
                        AnswerStatistic = answerStatistic.Value
                    });
                answeredQuestionsProbability += answerProbability;
            }
            foreach (var answerGuess in answerGuesses)
            {
                answerGuess.Probability /= answeredQuestionsProbability;
            }
            return answerGuesses.OrderByDescending(g => g.Probability).Take(maxGuesses).ToList();
        }

        public string GetNextQuestionId(IList<AnsweredQuestion> answeredQuestions, IList<AnswerGuess> answerGuesses)
        {
            string nextQuestionId = null;
            double minQuestionEntropy = double.MaxValue;
            var answeredQuestionIds = new HashSet<string>(answeredQuestions.Select(q => q.QuestionId));
            foreach (var questionId in questionStatistics.Keys)
            {
                if (answeredQuestionIds.Contains(questionId))
                    continue;
                double entropy = 0.0;
                for (int i = 0; i < answeringChoicesCount; i++)
                {
                    var answeredQuestion = new AnsweredQuestion { Choise = i, QuestionId = questionId };
                    var probabilities = answerGuesses.Select(ag => ag.Probability *
                                                                   GetAnsweringFrequency(ag.AnswerStatistic,answeredQuestion) /
                                                                   GetTotalAnsweringFrequency(ag.AnswerStatistic, answeredQuestion))
                                                                   .ToList();
                    var totalProbability = probabilities.Sum();
                    var answeredQuestionProbability = (double)questionStatistics[questionId].ChoicesFrequencies[i] /
                                                      questionStatistics[questionId].ChoicesFrequenciesTotal;
                    foreach (var probability in probabilities)
                    {
                        double correctedProbability = probability / totalProbability;
                        entropy += -correctedProbability * Math.Log(correctedProbability, 2.0) * answeredQuestionProbability;
                    }
                }
                if (entropy < minQuestionEntropy)
                {
                    minQuestionEntropy = entropy;
                    nextQuestionId = questionId;
                }
            }
            return nextQuestionId;
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

        public void UpdateStatisticsByAnswerGuessed(string answerId, IList<AnsweredQuestion> answeredQuestions)
        {
            answersGuessedCount++;
            if (!answerStatistics.ContainsKey(answerId))
            {
                answerStatistics.Add(answerId, new AnswerStatistic
                    {
                        AnswerCount = 1,
                        AnsweredQuestionsById = new Dictionary<string, QuestionStatistic>()
                    });
                foreach (var questionStatistic in questionStatistics)
                {
                    for (int i = 0; i < answeringChoicesCount; i++)
                    {
                        questionStatistic.Value.ChoicesFrequencies[i]++;
                        questionStatistic.Value.ChoicesFrequenciesTotal++;
                    }
                }
            }
            var answerStatistic = answerStatistics[answerId];
            answerStatistic.AnswerCount++;
            foreach (var answeredQuestion in answeredQuestions)
            {
                if (!answerStatistic.AnsweredQuestionsById.ContainsKey(answeredQuestion.QuestionId))
                    answerStatistic.AnsweredQuestionsById.Add(answeredQuestion.QuestionId, GetInitialQuestionStatistic());
                answerStatistic.AnsweredQuestionsById[answeredQuestion.QuestionId].ChoicesFrequencies[answeredQuestion.Choise]++;
                answerStatistic.AnsweredQuestionsById[answeredQuestion.QuestionId].ChoicesFrequenciesTotal++;
                questionStatistics[answeredQuestion.QuestionId].ChoicesFrequencies[answeredQuestion.Choise]++;
                questionStatistics[answeredQuestion.QuestionId].ChoicesFrequenciesTotal++;
            }
        }

        public void UpdateStatisticsByQuestionAdded(string questionId, IList<Tuple<string, int>> knownAnswers)
        {
            foreach (var knownAnswer in knownAnswers)
            {
                var answerStatistic = answerStatistics[knownAnswer.Item1];
                if (!answerStatistic.AnsweredQuestionsById.ContainsKey(questionId))
                    answerStatistic.AnsweredQuestionsById.Add(questionId, GetInitialQuestionStatistic());
                answerStatistic.AnsweredQuestionsById[questionId].ChoicesFrequencies[knownAnswer.Item2]++;
            }
            questionStatistics.Add(questionId,
                                   new QuestionStatistic { ChoicesFrequencies = knownAnswers.Select(ka => ka.Item2).Aggregate(new int[answeringChoicesCount],
                                                                                 (curr, i) =>
                                                                                     {
                                                                                         curr[i]++;
                                                                                         return curr;
                                                                                     })});
            questionStatistics[questionId].ChoicesFrequenciesTotal += answerStatistics.Count * answeringChoicesCount;
            for (int i = 0; i < answeringChoicesCount; i++)
                questionStatistics[questionId].ChoicesFrequencies[i] += answerStatistics.Count;
        }

        private QuestionStatistic GetInitialQuestionStatistic()
        {
            var frequencies = new int[answeringChoicesCount];
            for (int i = 0; i < answeringChoicesCount; i++)
                frequencies[i] = 1;
            return new QuestionStatistic{ ChoicesFrequencies = frequencies };
        }
    }
}
