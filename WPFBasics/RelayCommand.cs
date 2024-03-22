using System.Windows.Input;

namespace WPFBasics
{
    public class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
    {
        private readonly Action _execute = execute;
        private readonly Func<bool>? _canExecute = canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object? parameter)
        {
            _execute();
        }
    }

    public class RelayCommand<T>(Action<T?> execute, Func<T?, bool>? canExecute = null) : ICommand
    {
        private readonly Action<T?> _execute = execute;
        private readonly Func<T?, bool>? _canExecute = canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null
                || _canExecute(parameter == null ? default : (T)parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter == null ? default : (T)parameter);
        }
    }
}
