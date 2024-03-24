using System.Windows.Input;

namespace Core.MVVM
{
    public interface IEnchancedCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public class RelayCommand(Action execute, Func<bool>? canExecute = null) : IEnchancedCommand
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

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public class RelayCommand<T>(Action<T?> execute, Func<T?, bool>? canExecute = null) : IEnchancedCommand
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

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
