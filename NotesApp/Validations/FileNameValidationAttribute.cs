using NotesApp.Managers;
using NotesApp.Utils;
using Core.Validation;

namespace NotesApp.Validations
{
    public class FileNameValidationAttribute : ExtendedValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string fileName || string.IsNullOrWhiteSpace(fileName))
            {
                return Error("Name cannot be empty");
            }

            if (!PathHelper.IsFileNameValid(fileName, out HashSet<char> invalidChars))
            {
                return Error($"Name contains invalid characters: {string.Join("", invalidChars)}");
            }

            if (NoteManager.NoteFileExists(fileName))
            {
                return Error($"Note with this name already exists");
            }

            return true;
        }
    }
}
