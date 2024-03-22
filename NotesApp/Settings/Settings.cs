using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using NotesApp.Managers;

namespace NotesApp.Configuration
{
    [JsonObject(memberSerialization: MemberSerialization.OptIn)]
    public class Settings
    {
        [JsonProperty("ApplicationTheme")]
        private BaseTheme _theme;

        public event Action? SettingsChanged;

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

        public void ApplySettings()
        {
            ThemeManager.ApplyBaseTheme(Theme);
        }
    }
}
