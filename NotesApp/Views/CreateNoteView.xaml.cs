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
            var vm = new CreateNoteViewModel();
            vm.NoteCreated += OnNoteCreated;
            DataContext = vm;
        }

        private void OnNoteCreated(Note obj)
        {
            Note = obj;
            DialogResult = true;
            Close();
        }
    }
}
