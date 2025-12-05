using HCWpfFramework.Models;

namespace HCWpfFramework.Interfaces
{
    public interface IMessageService
    {
        void Subscribe(string recipientId, Action<IMessage> handler);
        void Unsubscribe(string recipientId);
        void SendMessage(IMessage message);
        void SendMessage(MessageType messageType, string senderId, object? content = null, string? targetId = null);
        IEnumerable<string> GetActiveSubscribers();
        event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public IMessage Message { get; }
        
        public MessageReceivedEventArgs(IMessage message)
        {
            Message = message;
        }
    }
}