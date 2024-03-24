using System.Windows;
using NotesApp.Models;
using NotesApp.ViewModels;

namespace NotesApp.Views
{
    /// <summary>
    /// Interaction logic for CreateNoteView.xaml
    /// </summary>
    public partial class CreateNoteView : Window
    {
        public Note? Note { get; private set; }

        public CreateNoteView()
        {
            InitializeComponent();
            CreateNoteViewModel viewModel = new();
            viewModel.NoteCreated += OnNoteCreated;
            DataContext = viewModel;
            _NoteName.Focus();
        }

        // todo: maybe rework to please the "design patterns" gods.
        private void OnNoteCreated(Note? obj)
        {
            Note = obj;
            DialogResult = Note != null;
            Close();
        }
    }
}
