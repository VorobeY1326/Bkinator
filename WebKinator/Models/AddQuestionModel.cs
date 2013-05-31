using System;
using System.Collections.Generic;
using System.Linq;

namespace WebKinator.Models
{
    public class AddQuestionModel
    {
        public static string[] ChoicesExtended;
        public static string DontknowName = "Не знаю ответа";

        static AddQuestionModel()
        {
            var choices = Choices.Names.ToList();
            choices.Add(DontknowName);
            ChoicesExtended = choices.ToArray();
        }

        public string Question { get; set; }
        public IList<Tuple<string,string>> AnswerIdsAndNames { get; set; }
        public IDictionary<string, string> Answers { get; set; } 
    }
}