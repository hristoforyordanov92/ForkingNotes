using NotesApp.Models;
using Core.MVVM;
using NotesApp.Validations;

namespace NotesApp.ViewModels
{
    public class CreateNoteViewModel : ViewModelBase
    {
        public event Action<Note?>? NoteCreated;

        public CreateNoteViewModel(bool renameMode)
        {
            // todo: ideally make a generic window which can be configured
            // so we avoid having to reuse this one while making it complex and weirdly coded
            RenameMode = renameMode;

            CreateNoteCommand = new RelayCommand(CreateNote, () => IsViewModelValid);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        public RelayCommand CreateNoteCommand { get; set; }
        public RelayCommand CloseWindowCommand { get; set; }

        public bool RenameMode { get; }

        private string? _fileName = string.Empty;
        [FileNameValidation]
        public string? FileName
        {
            get => _fileName;
            set
            {
                if (SetAndValidateField(ref _fileName, value))
                    CreateNoteCommand.RaiseCanExecuteChanged();
            }
        }

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
