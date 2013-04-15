using System.Collections.Generic;

namespace Bkinator
{
    public class AnswerStatistic
    {
        public string AnswerId { get; set; }
        public int AnswerCount { get; set; }
        public IDictionary<string, QuestionStatistic> AnsweredQuestionsById { get; set; }
    }
}