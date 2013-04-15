using ProtoBuf;

namespace Bkinator
{
    [ProtoContract]
    public class Question
    {
        [ProtoMember(1)]
        public string Text { get; set; }
    }
}