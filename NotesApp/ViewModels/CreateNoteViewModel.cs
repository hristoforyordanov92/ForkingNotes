using NotesApp.Models;
using Core.MVVM;

namespace NotesApp.ViewModels
{
    public class CreateNoteViewModel : ViewModelBase
    {
        public event Action<Note?>? NoteCreated;

        public CreateNoteViewModel()
        {
            CreateNoteCommand = new RelayCommand(CreateNote);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        public string? Name { get; set; }
        public string? Tags { get; set; }
        public string? Content { get; set; }

        public RelayCommand CreateNoteCommand { get; set; }
        public RelayCommand CloseWindowCommand { get; set; }

        private void CreateNote()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            var note = new Note(Name);

            NoteCreated?.Invoke(note);
        }

        private void CloseWindow()
        {
            NoteCreated?.Invoke(null);
        }
    }
}
