using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
    public abstract class ExtendedValidationAttribute : ValidationAttribute
    {
        public bool Error(string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                ErrorMessage = errorMessage;
            }

            return false;
        }
    }
}
