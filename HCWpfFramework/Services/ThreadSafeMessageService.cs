using System.Collections.Concurrent;
using System.Windows.Threading;
using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;

namespace HCWpfFramework.Services
{
    /// <summary>
    /// Thread-safe messaging service for inter-window communication in WPF
    /// </summary>
    public class ThreadSafeMessageService : IMessageService, IDisposable
    {
        private readonly ConcurrentDictionary<string, Action<IMessage>> _subscribers = new();
        private readonly object _eventLock = new();
        private volatile bool _disposed;

        /// <summary>
        /// Event raised when a message is received (thread-safe)
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        /// <summary>
        /// Singleton instance for global access
        /// </summary>
        public static ThreadSafeMessageService Instance { get; } = new ThreadSafeMessageService();

        /// <summary>
        /// Subscribe to receive messages for a specific recipient ID
        /// </summary>
        /// <param name="recipientId">The ID of the recipient</param>
        /// <param name="handler">The message handler</param>
        public void Subscribe(string recipientId, Action<IMessage> handler)
        {
            if (string.IsNullOrEmpty(recipientId))
                throw new ArgumentException("Recipient ID cannot be null or empty", nameof(recipientId));
            
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (_disposed)
                throw new ObjectDisposedException(nameof(ThreadSafeMessageService));

            // Wrap handler to ensure UI thread execution
            var wrappedHandler = CreateThreadSafeHandler(handler);
            _subscribers.AddOrUpdate(recipientId, wrappedHandler, (key, oldValue) => wrappedHandler);
        }

        /// <summary>
        /// Unsubscribe from receiving messages for a specific recipient ID
        /// </summary>
        /// <param name="recipientId">The ID of the recipient</param>
        public void Unsubscribe(string recipientId)
        {
            if (string.IsNullOrEmpty(recipientId))
                return;

            _subscribers.TryRemove(recipientId, out _);
        }

        /// <summary>
        /// Send a message to a specific target or broadcast to all subscribers
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendMessage(IMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (_disposed)
                return;

            // Raise the global event
            RaiseMessageReceived(message);

            // Send to specific target or broadcast
            if (!string.IsNullOrEmpty(message.TargetId))
            {
                // Send to specific target
                if (_subscribers.TryGetValue(message.TargetId, out var targetHandler))
                {
                    try
                    {
                        targetHandler(message);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in message handler for {message.TargetId}: {ex.Message}");
                    }
                }
            }
            else
            {
                // Broadcast to all subscribers
                var handlers = _subscribers.Values.ToArray();
                Parallel.ForEach(handlers, handler =>
                {
                    try
                    {
                        handler(message);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in broadcast message handler: {ex.Message}");
                    }
                });
            }
        }

        /// <summary>
        /// Send a message with simplified parameters
        /// </summary>
        /// <param name="messageType">Type of the message</param>
        /// <param name="senderId">ID of the sender</param>
        /// <param name="content">Message content</param>
        /// <param name="targetId">Target recipient ID (null for broadcast)</param>
        public void SendMessage(MessageType messageType, string senderId, object? content = null, string? targetId = null)
        {
            var message = new WindowMessage(messageType, senderId, content, targetId);
            SendMessage(message);
        }

        /// <summary>
        /// Get all active subscriber IDs
        /// </summary>
        /// <returns>Collection of subscriber IDs</returns>
        public IEnumerable<string> GetActiveSubscribers()
        {
            return _subscribers.Keys.ToArray();
        }

        /// <summary>
        /// Create a thread-safe wrapper for the message handler that uses WPF Dispatcher
        /// </summary>
        private Action<IMessage> CreateThreadSafeHandler(Action<IMessage> originalHandler)
        {
            // Get the current dispatcher
            var dispatcher = Dispatcher.CurrentDispatcher;
            
            return message =>
            {
                if (dispatcher.CheckAccess())
                {
                    // Already on UI thread
                    originalHandler(message);
                }
                else
                {
                    // Marshal to UI thread
                    dispatcher.BeginInvoke(originalHandler, message);
                }
            };
        }

        /// <summary>
        /// Safely raise the MessageReceived event
        /// </summary>
        private void RaiseMessageReceived(IMessage message)
        {
            EventHandler<MessageReceivedEventArgs>? handler;
            
            lock (_eventLock)
            {
                handler = MessageReceived;
            }

            if (handler != null)
            {
                var args = new MessageReceivedEventArgs(message);
                
                Task.Run(() =>
                {
                    try
                    {
                        handler(this, args);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in MessageReceived event handler: {ex.Message}");
                    }
                });
            }
        }

        /// <summary>
        /// Dispose of the messaging service
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _subscribers.Clear();
                
                lock (_eventLock)
                {
                    MessageReceived = null;
                }
            }
        }
    }
}