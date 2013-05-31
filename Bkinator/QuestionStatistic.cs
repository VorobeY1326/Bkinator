using System.Linq;
using ProtoBuf;

namespace Bkinator
{
    [ProtoContract]
    public class QuestionStatistic
    {
        [ProtoMember(1)]
        public int[] ChoicesFrequencies { get; set; }

        [ProtoMember(2)]
        private int? choicesFrequenciesTotalCached;
         // TODO methods that autoadd cached value
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