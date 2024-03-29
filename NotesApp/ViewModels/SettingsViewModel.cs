using System.Windows;
using Core.MVVM;
using MaterialDesignThemes.Wpf;
using NotesApp.Configuration;
using NotesApp.Managers;
using NotesApp.Utils;

namespace NotesApp.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Window _window;
        private readonly Settings _currentSettings;

        public SettingsViewModel(Window window)
        {
            _window = window;
            _currentSettings = SettingsManager.CurrentSettings;

            OpenSaveFolderCommand = new RelayCommand(OpenSaveFolder);
        }

        public RelayCommand OpenSaveFolderCommand { get; set; }

        public List<BaseTheme> ThemesCollection { get; set; } =
            [BaseTheme.Inherit, BaseTheme.Light, BaseTheme.Dark];

        /// <summary>
        /// The currently selected color theme.
        /// </summary>
        public BaseTheme SelectedTheme
        {
            get => _currentSettings.Theme;
            set => _currentSettings.Theme = value;
        }

        private void OpenSaveFolder()
        {
            PathHelper.OpenFolder(PathHelper.ToolSaveFolderPath);
        }
    }
}
