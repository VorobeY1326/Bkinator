using System;
using System.Collections.Generic;

namespace WebKinator.Models
{
    public class PLayModel
    {
        public string QuestionString { get; set; }
        public IList<Tuple<string,double>> Guesses { get; set; } 
    }
}