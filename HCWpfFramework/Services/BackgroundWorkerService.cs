using HCWpfFramework.Interfaces;
using HCWpfFramework.Models;

namespace HCWpfFramework.Services
{
    /// <summary>
    /// Example background worker that demonstrates async operations and messaging
    /// </summary>
    public class BackgroundWorkerService : IDisposable
    {
        private readonly string _workerId;
        private readonly IMessageService _messageService;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isRunning;
        private bool _disposed;

        public BackgroundWorkerService(string workerId, IMessageService messageService)
        {
            _workerId = workerId ?? throw new ArgumentNullException(nameof(workerId));
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        }

        public bool IsRunning => _isRunning;

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_isRunning || _disposed)
                return;

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _isRunning = true;

            _messageService.SendMessage(MessageType.Information, _workerId, "Background worker started");

            try
            {
                await DoWorkAsync(_cancellationTokenSource.Token);
                _messageService.SendMessage(MessageType.StatusChange, _workerId, "Background worker completed successfully");
            }
            catch (OperationCanceledException)
            {
                _messageService.SendMessage(MessageType.Information, _workerId, "Background worker was cancelled");
            }
            catch (Exception ex)
            {
                _messageService.SendMessage(MessageType.Error, _workerId, $"Background worker failed: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
            }
        }

        public void Stop()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                _messageService.SendMessage(MessageType.Information, _workerId, "Background worker stop requested");
            }
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            const int totalSteps = 10;
            
            for (int i = 0; i < totalSteps; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Simulate some work
                await Task.Delay(1000, cancellationToken);

                var progress = ((i + 1) * 100) / totalSteps;
                _messageService.SendMessage(MessageType.Information, _workerId, 
                    new { 
                        Message = $"Background worker progress: {progress}%",
                        Progress = progress,
                        Step = i + 1,
                        TotalSteps = totalSteps
                    });
            }

            // Simulate final processing
            await Task.Delay(500, cancellationToken);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _cancellationTokenSource?.Dispose();
                _disposed = true;
            }
        }
    }
}