using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly PaletteHelper _paletteHelper = new();

        private readonly Settings _currentSettings;

        //private BaseTheme _selectedTheme;

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
