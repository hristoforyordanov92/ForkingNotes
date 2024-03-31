using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Core.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string?>> _errors = [];

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errors.Values.Count > 0;

        private bool _isViewModelValid = false;
        /// <summary>
        /// Indicates if the view model has had its validations run and pass.
        /// Always starts as 'false' until at least a single validation has passed.
        /// </summary>
        public bool IsViewModelValid
        {
            get => _isViewModelValid && !HasErrors;
            private set => SetField(ref _isViewModelValid, value);
        }

        /// <summary>
        /// Sets a backing field. Raises PropertyChanged event when successfully set.
        /// </summary>
        /// <typeparam name="T">The type of the backing field.</typeparam>
        /// <param name="field">A reference to the backing field which will be set.</param>
        /// <param name="value">The value to set the backing field to.</param>
        /// <param name="propertyName">Name of the Property for which a PropertyChanged event will be raised.</param>
        /// <returns>True if the field is set properly, false otherwise.</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value))
                return false;

            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets and validates a backing field. Raises PropertyChanged event when successfully set.
        /// </summary>
        /// <typeparam name="T">The type of the backing field.</typeparam>
        /// <param name="field">A reference to the backing field which will be set.</param>
        /// <param name="value">The value to set the backing field to.</param>
        /// <param name="propertyName">Name of the Property for which a PropertyChanged event will be raised.</param>
        /// <returns>True if the field is set properly, false otherwise. Return result does not factor in the validation status of the backing field.</returns>
        protected bool SetAndValidateField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!SetField(ref field, value, propertyName))
                return false;

            Validate(field, propertyName);
            return true;
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

        public void Validate(object? propertyValue, [CallerMemberName] string propertyName = "")
        {
            List<ValidationResult> validationResults = [];

            Validator.TryValidateProperty(
                propertyValue,
                new ValidationContext(this)
                {
                    MemberName = propertyName
                },
                validationResults);

            if (validationResults.Count == 0)
            {
                _errors.Remove(propertyName);
            }
            else
            {
                List<string?> errors = new(validationResults.Count);
                errors.AddRange(validationResults.Select(r => r.ErrorMessage));
                _errors[propertyName] = errors;
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            RaisePropertyChanged(nameof(HasErrors));
            IsViewModelValid = !HasErrors;
        }
    }
}
