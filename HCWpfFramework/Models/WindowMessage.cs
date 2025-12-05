using HCWpfFramework.Interfaces;

namespace HCWpfFramework.Models
{
    public class WindowMessage : IMessage
    {
        public MessageType MessageType { get; }
        public string SenderId { get; }
        public object? Content { get; }
        public string? TargetId { get; }
        public DateTime Timestamp { get; }

        public WindowMessage(MessageType messageType, string senderId, object? content = null, string? targetId = null)
        {
            MessageType = messageType;
            SenderId = senderId ?? throw new ArgumentNullException(nameof(senderId));
            Content = content;
            TargetId = targetId;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {MessageType} from {SenderId}: {Content}";
        }
    }
}