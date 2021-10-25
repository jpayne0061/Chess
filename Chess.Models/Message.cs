using Chess.Models.Enums;

namespace Chess.Models
{
    public class Message
    {
        public Message(MessageType messageType)
        {
            MessageType = messageType;
        }
        public MessageType MessageType { get; }
        public object MessageContent { get; set; }
    }
}
