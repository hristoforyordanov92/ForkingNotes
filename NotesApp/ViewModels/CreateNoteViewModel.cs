using NotesApp.Models;
using Core.MVVM;
using NotesApp.Validations;

namespace NotesApp.ViewModels
{
    public class CreateNoteViewModel : ViewModelBase
    {
        public event Action<Note?>? NoteCreated;

        public CreateNoteViewModel()
        {
            CreateNoteCommand = new RelayCommand(CreateNote, () => IsViewModelValid);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        private string? _fileName = string.Empty;
        [FileNameValidation]
        public string? FileName
        {
            get => _fileName;
            set => SetAndValidateField(ref _fileName, value);
        }

        public RelayCommand CreateNoteCommand { get; set; }
        public RelayCommand CloseWindowCommand { get; set; }

        private void CreateNote()
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return;

            Note note = new(FileName);
            NoteCreated?.Invoke(note);
        }

        private void CloseWindow()
        {
            NoteCreated?.Invoke(null);
        }
    }
}
