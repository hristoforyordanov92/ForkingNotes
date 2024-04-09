using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace NotesApp.Validations
{
    public partial class TagValidation : ValidationRule
    {
        private static readonly Regex _tagRegex = TagRegex();

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is not string tag || string.IsNullOrEmpty(tag))
                return new ValidationResult(false, "Invalid tag");

            if (_tagRegex.Match(tag).Success)
                return new ValidationResult(false, "Tag must contain only lowercase letters, digits and -.");

            return ValidationResult.ValidResult;
        }

        [GeneratedRegex(@"[^a-z0-9\-]")]
        private static partial Regex TagRegex();
    }
}
