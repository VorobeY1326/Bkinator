using ProtoBuf;

namespace Bkinator
{
    [ProtoContract]
    public class Answer
    {
        [ProtoMember(1)]
        public string Text { get; set; }
        [ProtoMember(2)]
        public string Description { get; set; }
    }
}