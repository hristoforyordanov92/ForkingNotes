namespace Core.MVVM
{
    public class RelayCommand(Action execute, Func<bool>? canExecute = null)
        : IExtendedCommand
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

    public class RelayCommand<T>(Action<T> execute, Func<T, bool>? canExecute = null)
        : IExtendedCommand
    {
        private readonly Action<T> _execute = execute;
        private readonly Func<T, bool>? _canExecute = canExecute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter), $"A parameter with a 'null' value has been passed into the {nameof(CanExecute)} method of this command");

            return _canExecute == null
                || _canExecute((T)parameter);
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter), $"A parameter with a 'null' value has been passed into the {nameof(Execute)} method of this command");

            _execute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
