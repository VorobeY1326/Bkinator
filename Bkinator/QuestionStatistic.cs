using System.Linq;

namespace Bkinator
{
    public class QuestionStatistic
    {
        public int[] ChoicesFrequencies { get; set; }

        private int? choicesFrequenciesTotalCached;

        public int ChoicesFrequenciesTotal
        {
            get
            {
                if (!choicesFrequenciesTotalCached.HasValue)
                    choicesFrequenciesTotalCached = ChoicesFrequencies.Sum();
                return choicesFrequenciesTotalCached.Value;
            }
            set { choicesFrequenciesTotalCached = value; }
        }
    }
}