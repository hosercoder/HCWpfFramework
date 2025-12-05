namespace HCWpfFramework.Interfaces
{
    public interface IMessage
    {
        Models.MessageType MessageType { get; }
        string SenderId { get; }
        object? Content { get; }
        string? TargetId { get; }
        DateTime Timestamp { get; }
    }
}