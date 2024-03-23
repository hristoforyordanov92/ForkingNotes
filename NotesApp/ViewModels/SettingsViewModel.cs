using System.Windows;
using Core.MVVM;
using MaterialDesignThemes.Wpf;
using NotesApp.Configuration;
using NotesApp.Managers;

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
        }

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
    }
}
