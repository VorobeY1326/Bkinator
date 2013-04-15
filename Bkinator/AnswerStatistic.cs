using System.Collections.Generic;
using ProtoBuf;

namespace Bkinator
{
    [ProtoContract]
    public class AnswerStatistic
    {
        [ProtoMember(1)]
        public string AnswerId { get; set; }
        [ProtoMember(2)]
        public int AnswerCount { get; set; }
        [ProtoMember(3)]
        public IDictionary<string, QuestionStatistic> AnsweredQuestionsById { get; set; }
    }
}