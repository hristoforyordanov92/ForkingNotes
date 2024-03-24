using System.ComponentModel;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using NotesApp.Managers;

namespace NotesApp.Configuration
{
    [JsonObject(memberSerialization: MemberSerialization.OptIn)]
    public class Settings
    {
        public event Action? SettingsChanged;

        #region Theme Settings

        [JsonProperty("ApplicationTheme")]
        private BaseTheme _theme = BaseTheme.Inherit;
        public BaseTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                ThemeManager.ApplyBaseTheme(_theme);
                SettingsChanged?.Invoke();
            }
        }

        #endregion

        #region Main Window Settings

        [JsonProperty(nameof(MainWindowLeftColumnWidth))]
        private double _mainWindowLeftColumnWidth = 1;
        public double MainWindowLeftColumnWidth
        {
            get => _mainWindowLeftColumnWidth;
            set
            {
                _mainWindowLeftColumnWidth = value;
                SettingsChanged?.Invoke();
            }
        }

        [JsonProperty(nameof(MainWindowRightColumnWidth))]
        private double _mainWindowRightColumnWidth = 4;
        public double MainWindowRightColumnWidth
        {
            get => _mainWindowRightColumnWidth;
            set
            {
                _mainWindowRightColumnWidth = value;
                SettingsChanged?.Invoke();
            }
        }

        [JsonProperty(nameof(MainWindowWidth))]
        private double _mainWindowWidth = 1200;
        public double MainWindowWidth
        {
            get => _mainWindowWidth;
            set
            {
                _mainWindowWidth = value;
                SettingsChanged?.Invoke();
            }
        }

        [JsonProperty(nameof(MainWindowHeight))]
        private double _mainWindowHeight = 600;
        public double MainWindowHeight
        {
            get => _mainWindowHeight;
            set
            {
                _mainWindowHeight = value;
                SettingsChanged?.Invoke();
            }
        }

        [JsonProperty(nameof(MainWindowIsMaximized))]
        private bool _mainWindowIsMaximized = false;
        public bool MainWindowIsMaximized
        {
            get => _mainWindowIsMaximized;
            set
            {
                _mainWindowIsMaximized = value;
                SettingsChanged?.Invoke();
            }
        }

        #endregion

        // todo: add a way to change the folder in which the application will save the notes and settings.
        // todo: also when the folder is changed, allow the user to also move the old file to the new folder
        // todo: maybe we'll need to make something like Obsidian, where they make a Vault for each "project".
        // so we can save the settings in the Vault folder. but this could be problematic, because we might need global settings...

        public void ApplySettings()
        {
            ThemeManager.ApplyBaseTheme(Theme);
        }
    }
}
