using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Core.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string?>> _errors = [];

        /// <summary>
        /// Dictionary containing property names and commands, which are dependent on the properties' validations.
        /// </summary>
        private readonly Dictionary<string, IEnchancedCommand> _propertyDependents = [];

        public bool HasErrors => _errors.Count > 0;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value))
                return false;

            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected bool SetAndValidateField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (SetField(ref field, value, propertyName))
            {
                Validate(field, propertyName);
                return true;
            }

            return false;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertiesChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.TryGetValue(propertyName, out List<string?>? errors))
                return Enumerable.Empty<string>();

            return errors;
        }

        protected bool IsValid(string propertyName)
        {
            return _errors.TryGetValue(propertyName, out List<string?>? errors) && errors.Count == 0;
        }

        protected bool HasError(string propertyName)
        {
            return !IsValid(propertyName);
        }

        public void Validate(object? propertyValue, [CallerMemberName] string propertyName = "")
        {
            List<ValidationResult> results = [];

            Validator.TryValidateProperty(
                propertyValue,
                new ValidationContext(this)
                {
                    MemberName = propertyName
                },
                results);

            if (!_errors.ContainsKey(propertyName))
                _errors.Add(propertyName, []);

            _errors[propertyName].Clear();
            if (results.Count > 0)
            {
                _errors[propertyName].AddRange(results.Select(r => r.ErrorMessage));
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            if (_propertyDependents.TryGetValue(propertyName, out IEnchancedCommand? command))
                command.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Register a command as dependent on a collection of properties. Whenever the properties get validated,
        /// the command's CanExecute function will be automatically executed.
        /// </summary>
        /// <param name="command">The command to register as dependent.</param>
        /// <param name="properties">The properties which will trigger the command's CanExecute reevaluation.</param>
        protected void RegisterPropertiesDependency(IEnchancedCommand command, params string[] properties)
        {
            foreach (var property in properties)
                _propertyDependents.Add(property, command);
        }
    }
}
