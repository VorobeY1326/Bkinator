using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Bkinator
{
    [ProtoContract]
    public class BkinatorState
    {
        [ProtoMember(1)]
        public Genie Genie { get; private set; }
        [ProtoMember(2)]
        public IDictionary<string, Answer> Answers { get; private set; }
        [ProtoMember(3)]
        public IDictionary<string, Question> Questions { get; private set; }

// ReSharper disable UnusedMember.Local
        private BkinatorState()
        {}
// ReSharper restore UnusedMember.Local

        public BkinatorState(Genie genie, IDictionary<string, Answer> answers, IDictionary<string, Question> questions)
        {
            Genie = genie;
            Answers = answers;
            Questions = questions;
        }

        public string AddNewAnswer(Answer answer, IList<AnsweredQuestion> answeredQuestions)
        {
            var answerId = Guid.NewGuid().ToString();
            Answers.Add(answerId, answer);
            Genie.UpdateStatisticsByAnswerGuessed(answerId, answeredQuestions);
            return answerId;
        }

        public string AddNewQuestion(Question question, IList<Tuple<string, int>> knownAnswers)
        {
            var questionId = Guid.NewGuid().ToString();
            Questions.Add(questionId, question);
            Genie.UpdateStatisticsByQuestionAdded(questionId, knownAnswers);
            return questionId;
        }

        public void SaveToStream(Stream stream)
        {
            Serializer.Serialize(stream, this);
        }

        public static BkinatorState LoadFromStream(Stream stream)
        {
            return Serializer.Deserialize<BkinatorState>(stream);
        }
    }
}