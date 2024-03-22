using System.Windows;
using NotesApp.ViewModels;

namespace NotesApp.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel(this);
        }
    }
}
