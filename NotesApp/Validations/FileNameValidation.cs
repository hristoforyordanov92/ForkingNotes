using System.Globalization;
using System.Windows.Controls;
using NotesApp.Utils;

namespace NotesApp.Validations
{
    public class FileNameValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is not string name || string.IsNullOrWhiteSpace(name))
            {
                return new ValidationResult(false, "Name cannot be empty");
            }

            if (!PathHelper.IsFileNameValid(name, out HashSet<char> invalidChars))
            {
                return new ValidationResult(false, $"Name contains invalid characters: {string.Join("", invalidChars)}");
            }

            return ValidationResult.ValidResult;
        }
    }
}
