using System.Windows;
using NotesApp.Managers;

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SettingsManager.LoadApplicationSettings();
        }
    }
}
