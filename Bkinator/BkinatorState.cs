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