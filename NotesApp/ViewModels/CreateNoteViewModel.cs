using System.Windows;
using NotesApp.Models;
using WPFBasics;

namespace NotesApp.ViewModels
{
    public class CreateNoteViewModel : ViewModelBase
    {
        public event Action<Note>? NoteCreated;

        public CreateNoteViewModel()
        {
            CreateNoteCommand = new RelayCommand(CreateNote);
        }

        public string? Name { get; set; }
        public string? Tags { get; set; }
        public string? Content { get; set; }

        public RelayCommand CreateNoteCommand { get; set; }

        private void CreateNote()
        {
            // todo: need to implement the logic and some validation
            if (string.IsNullOrWhiteSpace(Name)
                || string.IsNullOrWhiteSpace(Content)
                || string.IsNullOrWhiteSpace(Tags))
                return;

            var note = new Note(Name)
            {
                Content = Content,
                Tags = [.. Tags.Split(' ')]
            };

            NoteCreated?.Invoke(note);
        }
    }
}
