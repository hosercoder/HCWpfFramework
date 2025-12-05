using System.Windows.Input;

namespace HCWpfFramework.Commands
{
    /// <summary>
    /// Non-generic RelayCommand implementation
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            
            _execute = _ => execute();
            _canExecute = canExecute != null ? _ => canExecute() : null;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    /// Generic RelayCommand implementation for type-safe parameter handling
    /// </summary>
    /// <typeparam name="T">The type of the command parameter</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null && !typeof(T).IsValueType)
                return _canExecute?.Invoke(default(T)) ?? true;
                
            if (parameter is T typedParameter)
                return _canExecute?.Invoke(typedParameter) ?? true;
                
            // Try to convert the parameter
            try
            {
                var convertedParameter = (T?)Convert.ChangeType(parameter, typeof(T));
                return _canExecute?.Invoke(convertedParameter) ?? true;
            }
            catch
            {
                return false;
            }
        }

        public void Execute(object? parameter)
        {
            if (parameter == null && !typeof(T).IsValueType)
            {
                _execute(default(T));
                return;
            }
                
            if (parameter is T typedParameter)
            {
                _execute(typedParameter);
                return;
            }
                
            // Try to convert the parameter
            try
            {
                var convertedParameter = (T?)Convert.ChangeType(parameter, typeof(T));
                _execute(convertedParameter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot convert parameter of type {parameter?.GetType().Name} to {typeof(T).Name}", ex);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}