using System.Globalization;
using System.Windows.Controls;

namespace NotesApp.Validations
{
    public partial class TagValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is not string tag || string.IsNullOrWhiteSpace(tag))
                return new ValidationResult(false, "Invalid tag");

            return ValidationResult.ValidResult;
        }
    }
}
